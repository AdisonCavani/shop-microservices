using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CoreShared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Contracts.Dtos;
using OrderService.Contracts.Requests;
using OrderService.Database;
using OrderService.Database.Entities;
using ProductService;
using ProtobufSpec;
using Stripe.Checkout;

namespace OrderService.Endpoints.Payment;

public static class Create
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, BadRequest<string>, Created<PaymentDto>>> HandleAsync(
        HttpContext httpContext,
        [FromBody] CreatePaymentReq req,
        [FromServices] ProductAPI.ProductAPIClient client,
        [FromServices] AppDbContext dbContext)
    {
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = httpContext.User.FindFirstValue(ClaimTypes.Email);
        
        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        if (userEmail is null)
            throw new Exception(ExceptionMessages.EmailNull);
        
        var userId = Guid.Parse(userIdStr);
        
        var order = await dbContext.Orders
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == req.OrderId && x.UserId == userId);

        if (order is null)
            return TypedResults.NotFound();

        var alreadyPaid = order.Payments.Any(x => x.Paid);
        
        if (alreadyPaid)
            return TypedResults.BadRequest("Order is already paid");

        var validPaymentExists = order.Payments.Any(x => x.ExpiresAt > DateTime.UtcNow);
        
        if (validPaymentExists)
            return TypedResults.BadRequest("Pending payment already exists");

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

        return TypedResults.Created($"{ApiRoutes.Payment.Path}/{order.ProductId}", new PaymentDto
        {
            PaymentUrl = checkoutSession.Url
        });
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Create payment for order";

        return operation;
    }
}