using System.Diagnostics.CodeAnalysis;
using Gateway.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Gateway.Endpoints.User;

public static class VerifyEmail
{
    internal static async Task<Results<StatusCodeHttpResult, Ok>> HandleAsync(
        HttpContext httpContext,
        [FromRoute] Guid token,
        [FromServices] IUserRepository repository)
    {
        await repository.VerifyEmailAsync(token);
        return TypedResults.Ok();
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Verify user's email";

        return operation;
    }
}