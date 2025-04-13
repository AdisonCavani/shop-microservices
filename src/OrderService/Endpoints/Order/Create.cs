using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CoreShared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OrderService.Contracts.Dtos;
using OrderService.Contracts.Requests;
using OrderService.Database;
using OrderService.Database.Entities;
using OrderService.Mappers;
using ProductService;
using ProtobufSpec;

namespace OrderService.Endpoints.Order;

public static class Create
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, Created<OrderDto>>> HandleAsync(
        [FromBody] CreateOrderReq req,
        HttpContext httpContext,
        [FromServices] ProductAPI.ProductAPIClient client,
        [FromServices] AppDbContext dbContext)
    {
        var response = await client.GetProductAsync(new GetProductReq
        {
            Id = req.ProductId.ToString(),
        });

        if (response is null)
            return TypedResults.NotFound();
        
        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);
        
        var userId = Guid.Parse(userIdStr);
        
        var orderEntity = new OrderEntity
        {
            UserId = userId,
            ProductId = req.ProductId,
            Quantity = req.Quantity,
        };
        
        dbContext.Orders.Add(orderEntity);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"{ApiRoutes.Product.Path}/{orderEntity.Id}", orderEntity.ToOrderDto());
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Create order";

        return operation;
    }
}