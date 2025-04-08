using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProductService.Contracts.Dtos;
using ProductService.Contracts.Requests;
using ProtobufSpec;

namespace ProductService.Endpoints.Product;

public static class Create
{
    internal static async Task<Results<StatusCodeHttpResult, Created<ProductDto>>> HandleAsync(
        [FromBody] CreateProductReq req,
        HttpContext context)
    {
        var product = new ProductDto
        {
            Name = "",
           Description = "",
           Price = 0,
           Category = new()
           {
               Name = ""
           }
        };
        
        return TypedResults.Created($"{ApiRoutes.Product.BasePath}/{product.Id}" , product);
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Create product endpoint";

        return operation;
    }
}