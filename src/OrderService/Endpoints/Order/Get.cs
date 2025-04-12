using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Contracts.Dtos;
using OrderService.Database;
using OrderService.Mappers;

namespace OrderService.Endpoints.Order;

public static class Get
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, Ok<OrderDto>>> HandleAsync(
        [FromRoute] Guid orderId,
        [FromServices] AppDbContext context)
    {
        // TODO: use real userId
        var orderEntity = await context.Orders.FirstOrDefaultAsync(x => x.UserId == Guid.NewGuid() && x.Id == orderId);

        if (orderEntity is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(orderEntity.ToOrderDto());
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get order by id";

        return operation;
    }
}