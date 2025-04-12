using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Contracts.Responses;
using OrderService.Database;
using OrderService.Mappers;

namespace OrderService.Endpoints.Order;

public static class List
{
    internal static async Task<Results<StatusCodeHttpResult, Ok<ListOrdersRes>>> HandleAsync(
        [FromServices] AppDbContext dbContext)
    {
        // TODO: use real userId
        var orderEntities = await dbContext.Orders
            .Where(x => x.UserId == Guid.NewGuid())
            .ToListAsync();
        
        var orders = orderEntities.Select(x => x.ToOrderDto());
        
        return TypedResults.Ok(new ListOrdersRes { Orders = orders });
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get all orders";

        return operation;
    }
}