using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CoreShared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OrderService.Contracts.Dtos;
using OrderService.Contracts.Requests;
using OrderService.Services;
using ProtobufSpec;

namespace OrderService.Endpoints.Payment;

public static class Create
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, Created<PaymentDto>>> HandleAsync(
        HttpContext httpContext,
        [FromBody] CreatePaymentReq req,
        [FromServices] IPaymentService paymentService)
    {
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = httpContext.User.FindFirstValue(ClaimTypes.Email);
        
        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        if (userEmail is null)
            throw new Exception(ExceptionMessages.EmailNull);
        
        var userId = Guid.Parse(userIdStr);
        
        var response = await paymentService.CreatePaymentAsync(req.OrderId, userId, userEmail);

        if (response is null)
            TypedResults.NotFound();

        var (productId, paymentDto) = response!;
        return TypedResults.Created($"{ApiRoutes.Payment.Path}/{productId}", paymentDto);
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Create payment for order";

        return operation;
    }
}