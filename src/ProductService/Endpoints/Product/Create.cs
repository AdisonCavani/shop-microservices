using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProductService.Contracts.Requests;
using ProductService.Services;
using ProtobufSpec;

namespace ProductService.Endpoints.Product;

public static class Create
{
    internal static async Task<Results<StatusCodeHttpResult, Created<ProductDto>>> HandleAsync(
        [FromBody] CreateProductReq req,
        [FromServices] IProductService productService)
    {
        var product = await productService.CreateProductAsync(req.Name, req.Description, req.PriceCents, req.ActivationCode);

        return TypedResults.Created($"{ApiRoutes.Product.Path}/{product.Id}", product);
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Create product";

        return operation;
    }
}