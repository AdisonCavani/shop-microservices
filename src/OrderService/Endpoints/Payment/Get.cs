using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CoreShared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OrderService.Contracts.Dtos;
using OrderService.Services;

namespace OrderService.Endpoints.Payment;

public static class Get
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, Ok<PaymentDto>>> HandleAsync(
        [FromRoute] Guid orderId,
        HttpContext httpContext,
        [FromServices] IPaymentService paymentService)
    {
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        var userId = Guid.Parse(userIdStr);
        
        var paymentDto = await paymentService.GetPaymentAsync(orderId, userId);

        if (paymentDto is null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(paymentDto);
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get payment for order";

        return operation;
    }
}