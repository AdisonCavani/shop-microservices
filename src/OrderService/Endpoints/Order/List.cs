using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CoreShared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OrderService.Contracts.Responses;
using OrderService.Services;

namespace OrderService.Endpoints.Order;

public static class List
{
    internal static async Task<Results<StatusCodeHttpResult, Ok<ListOrdersRes>>> HandleAsync(
        HttpContext httpContext,
        [FromServices] IOrderService orderService)
    {
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        var userId = Guid.Parse(userIdStr);
        
        var orders = await orderService.GetOrdersAsync(userId);
        
        return TypedResults.Ok(new ListOrdersRes { Orders = orders });
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get all orders";

        return operation;
    }
}