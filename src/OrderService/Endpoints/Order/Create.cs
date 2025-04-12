using System.Diagnostics.CodeAnalysis;
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
        [FromServices] ProductAPI.ProductAPIClient client,
        [FromServices] AppDbContext context)
    {
        var response = await client.GetProductAsync(new GetProductReq
        {
            Id = req.ProductId.ToString(),
        });

        if (response is null)
            return TypedResults.NotFound();
        
        var orderEntity = new OrderEntity
        {
            UserId = Guid.NewGuid(), // TODO: add auth
            ProductId = req.ProductId,
            Quantity = req.Quantity,
        };
        
        context.Orders.Add(orderEntity);
        await context.SaveChangesAsync();

        return TypedResults.Created($"{ApiRoutes.Product.Path}/{orderEntity.Id}", orderEntity.ToOrderDto());
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Create order";

        return operation;
    }
}