using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CoreShared;
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
        HttpContext httpContext,
        [FromServices] AppDbContext dbContext)
    {
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        var userId = Guid.Parse(userIdStr);
        
        var orderEntities = await dbContext.Orders
            .Where(x => x.UserId == userId)
            .Include(x => x.Payments)
            .ToListAsync();
        
        var orders = orderEntities
            .Select(x => x.ToOrderDto(x.Payments
                .FirstOrDefault(y => y.Paid || (!y.Paid && y.ExpiresAt > DateTime.UtcNow))));
        
        return TypedResults.Ok(new ListOrdersRes { Orders = orders });
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get all orders";

        return operation;
    }
}