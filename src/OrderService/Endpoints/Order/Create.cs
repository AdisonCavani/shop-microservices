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

namespace OrderService.Endpoints.Order;

public static class Create
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, Created<OrderDto>>> HandleAsync(
        [FromBody] CreateOrderReq req,
        HttpContext httpContext,
        [FromServices] IOrderService orderService)
    {
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        var userId = Guid.Parse(userIdStr);

        var orderDto = await orderService.CreateOrderAsync(req.ProductId, userId);
        
        if (orderDto is null)
            return TypedResults.NotFound();
        
        return TypedResults.Created($"{ApiRoutes.Order.Path}/{orderDto.Id}", orderDto);
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Create order";

        return operation;
    }
}