using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Server.Contracts.Requests;

namespace Server.Endpoints.User;

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