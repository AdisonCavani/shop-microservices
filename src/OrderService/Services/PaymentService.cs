using CoreShared;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Contracts.Dtos;
using OrderService.Database;
using OrderService.Database.Entities;
using ProductService;
using ProtobufSpec.Events;
using Stripe.Checkout;

namespace OrderService.Services;

public class PaymentService(
    AppDbContext dbContext,
    ProductAPI.ProductAPIClient client,
    IBus bus) : IPaymentService
{
    public async Task<Tuple<Guid, PaymentDto>?> CreatePaymentAsync(Guid orderId, Guid userId, string userEmail)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);

        if (order is null)
            return null;

        var alreadyPaid = order.Payments.Any(x => x.Paid);
        
        if (alreadyPaid)
            throw new ProblemException(ExceptionMessages.OrderAlreadyPaid, "You cannot pay twice for the same order");

        var validPaymentExists = order.Payments.Any(x => x.ExpiresAt > DateTime.UtcNow);
        
        if (validPaymentExists)
            throw new ProblemException(ExceptionMessages.PaymentAlreadyCreated, "Only one non-expired payment is allowed");

        var product = await client.GetProductAsync(new GetProductReq
        {
            Id = order.ProductId.ToString()
        });

        if (product is null)
            throw new Exception(ExceptionMessages.ProductLost);

        var lineItems = new List<SessionLineItemOptions>
        {
            new()
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = product.PriceCents,
                    Currency = "PLN",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Name,
                        Description = product.Description
                    }
                },
                Quantity = 1
            }
        };
        
        var paymentId = Guid.NewGuid();
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            InvoiceCreation = new()
            {
                Enabled = true
            },
            SuccessUrl = "https://localhost/success",
            CancelUrl = "https://localhost/cancel",
            LineItems = lineItems,
            CustomerEmail = userEmail,
            PaymentIntentData = new()
            {
                ReceiptEmail = userEmail,
                Metadata = new Dictionary<string, string>
                {
                    {"PaymentId", paymentId.ToString()}
                }
            }
        };
        
        var checkoutSession = await new SessionService().CreateAsync(options);

        dbContext.Payments.Add(new PaymentEntity
        {
            Id = paymentId,
            StripeCheckoutId = checkoutSession.Id,
            CreatedAt = checkoutSession.Created,
            ExpiresAt = checkoutSession.ExpiresAt,
            OrderId = order.Id
        });
        await dbContext.SaveChangesAsync();

        return new(order.ProductId, new()
        {
            PaymentUrl = checkoutSession.Url,
            ExpirationDate = checkoutSession.ExpiresAt
        });
    }

    public async Task<PaymentDto?> GetPaymentAsync(Guid orderId, Guid userId)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(orderEntity => orderEntity.Payments)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == orderId);

        if (order is null)
            return null;

        var payment = order.Payments.FirstOrDefault(x => x.Paid || x.ExpiresAt > DateTime.UtcNow);

        if (payment is null)
            return null;

        if (payment.Paid)
            return new PaymentDto
            {
                Paid = true
            };
        
        var paymentSession = await new SessionService().GetAsync(payment.StripeCheckoutId);

        if (paymentSession is null)
            return null;

        if ((!payment.Paid && paymentSession.PaymentStatus == "paid")
            || (payment.Paid && paymentSession.PaymentStatus != "paid"))
            throw new Exception(ExceptionMessages.PaymentMismatch);
        
        return new PaymentDto
        {
            ExpirationDate = payment.ExpiresAt,
            PaymentUrl = paymentSession.Url
        };
    }

    public async Task PaymentReceivedAsync(PaymentSucceededEvent message)
    {
        var payment = await dbContext.Payments
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.Id == message.PaymentId);

        if (payment is null)
            throw new Exception(ExceptionMessages.PaymentLost);

        payment.Paid = true;
        await dbContext.SaveChangesAsync();

        await bus.Publish(new OrderCompletedEvent
        {
            OrderId = payment.Order.Id,
            ProductId = payment.Order.ProductId,
            UserId = payment.Order.UserId
        });
    }
}