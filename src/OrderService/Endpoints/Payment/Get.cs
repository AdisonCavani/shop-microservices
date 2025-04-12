using System.Diagnostics.CodeAnalysis;
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
        [FromServices] AppDbContext dbContext)
    {
        // TODO: use real userId
        var order = await dbContext.Orders
            .Include(orderEntity => orderEntity.Payments)
            .FirstOrDefaultAsync(x => x.UserId == Guid.NewGuid() && x.Id == orderId);

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