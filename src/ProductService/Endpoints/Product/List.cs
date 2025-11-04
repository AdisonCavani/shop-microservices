using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProductService.Contracts.Responses;
using ProductService.Services;

namespace ProductService.Endpoints.Product;

public static class List
{
    internal static async Task<Results<StatusCodeHttpResult, Ok<ListProductsRes>>> HandleAsync(
        [FromServices] IProductService productService)
    {
        var products = await productService.GetProductsAsync();

        return TypedResults.Ok(new ListProductsRes { Products = products });
    }
    
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get all products";

        return operation;
    }
}