using System.Diagnostics.CodeAnalysis;
using Gateway.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Gateway.Endpoints.User;

public static class Logout
{
    internal static async Task<Results<StatusCodeHttpResult, Ok>> HandleAsync(
        [FromBody] VerifyEmailReq req,
        HttpContext context)
    {
        await context.SignOutAsync();
        return TypedResults.Ok();
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Logout endpoint";

        return operation;
    }
}