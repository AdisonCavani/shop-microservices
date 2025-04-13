using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CoreShared;
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
        HttpContext httpContext,
        [FromServices] AppDbContext dbContext)
    {
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        var userId = Guid.Parse(userIdStr);
        
        var orderEntity = await dbContext.Orders
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == orderId);

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