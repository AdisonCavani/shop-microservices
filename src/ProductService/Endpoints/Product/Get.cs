using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProductService.Services;

namespace ProductService.Endpoints.Product;

public static class Get
{
    internal static async Task<Results<StatusCodeHttpResult, NotFound, Ok<ProductDto>>> HandleAsync(
        [FromRoute] Guid productId,
        [FromServices] IProductService productService)
    {
        var product = await productService.GetProductAsync(productId);

        if (product is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(product);
    }
    
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get product by id";

        return operation;
    }
}