using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CoreShared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Contracts.Dtos;
using OrderService.Database;
using Stripe.Checkout;

namespace OrderService.Endpoints.Payment;

public static class Get
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, NoContent, Ok<PaymentDto>>> HandleAsync(
        [FromRoute] Guid orderId,
        HttpContext httpContext,
        [FromServices] AppDbContext dbContext)
    {
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        var userId = Guid.Parse(userIdStr);
        
        var order = await dbContext.Orders
            .Include(orderEntity => orderEntity.Payments)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == orderId);

        if (order is null)
            return TypedResults.NotFound();

        var payment = order.Payments.FirstOrDefault(x => x.ExpiresAt > DateTime.Now);
        
        if (payment is null)
            return TypedResults.NotFound();

        if (payment.Paid)
            return TypedResults.NoContent();
        
        var paymentSession = await new SessionService().GetAsync(payment.StripeCheckoutId);

        if (paymentSession is null)
            return TypedResults.NotFound();

        if ((!payment.Paid && paymentSession.PaymentStatus == "paid")
            || (payment.Paid && paymentSession.PaymentStatus != "paid"))
            throw new Exception(ExceptionMessages.PaymentMismatch);
        
        return TypedResults.Ok(new PaymentDto
        {
            PaymentUrl = paymentSession.Url
        });
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get payment for order";

        return operation;
    }
}