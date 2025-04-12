using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;

namespace Gateway.Endpoints.User;

public static class Logout
{
    internal static async Task<Results<StatusCodeHttpResult, Ok>> HandleAsync(HttpContext httpContext)
    {
        await httpContext.SignOutAsync();
        return TypedResults.Ok();
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Logout";

        return operation;
    }
}