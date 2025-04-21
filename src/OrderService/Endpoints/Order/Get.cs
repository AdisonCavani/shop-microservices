using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CoreShared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OrderService.Contracts.Dtos;
using OrderService.Services;

namespace OrderService.Endpoints.Order;

public static class Get
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, Ok<OrderDto>>> HandleAsync(
        [FromRoute] Guid orderId,
        HttpContext httpContext,
        [FromServices] IOrderService orderService)
    {
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        var userId = Guid.Parse(userIdStr);

        var orderDto = await orderService.GetOrderAsync(orderId, userId);

        if (orderDto is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(orderDto);
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get order by id";

        return operation;
    }
}