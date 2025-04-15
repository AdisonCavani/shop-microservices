using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductService.Database;
using ProductService.Mappers;

namespace ProductService.Endpoints.Product;

public static class Get
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, Ok<ProductDto>>> HandleAsync(
        [FromRoute] Guid productId,
        [FromServices] AppDbContext dbContext)
    {
        var productEntity = await dbContext.Products
            .FirstOrDefaultAsync(x => x.CompletedOrderId == null && x.Id == productId);

        if (productEntity is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(productEntity.ToProductDto());
    }
    
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get product by id";

        return operation;
    }
}