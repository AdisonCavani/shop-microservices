using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Server.Contracts.Requests;
using Server.Repositories;

namespace Server.Endpoints.User;

public static class VerifyEmail
{
    internal static async Task<Results<StatusCodeHttpResult, Ok>> HandleAsync(
        [FromBody] VerifyEmailReq req,
        [FromServices] IUserRepository repository)
    {
        await repository.VerifyEmailAsync(req);
        return TypedResults.Ok();
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Verify user's email endpoint";

        return operation;
    }
}