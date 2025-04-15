using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductService.Contracts.Responses;
using ProductService.Database;
using ProductService.Mappers;

namespace ProductService.Endpoints.Product;

public static class List
{
    internal static async Task<Results<StatusCodeHttpResult, Ok<ListProductsRes>>> HandleAsync(
        [FromServices] AppDbContext dbContext)
    {
        var productsEntities = await dbContext.Products
            .Where(x => x.CompletedOrderId == null).ToListAsync();
        
        var products = productsEntities.Select(x => x.ToProductDto());

        return TypedResults.Ok(new ListProductsRes { Products = products });
    }
    
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get all products";

        return operation;
    }
}