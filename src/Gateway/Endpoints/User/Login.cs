using System.Diagnostics.CodeAnalysis;
using Gateway.Contracts.Requests;
using Gateway.Contracts.Responses;
using Gateway.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Gateway.Endpoints.User;

public static class Login
{
    internal static async Task<Results<StatusCodeHttpResult, Ok<LoginRes>>> HandleAsync(
        [FromBody] LoginReq req,
        HttpContext context,
        [FromServices] IUserRepository repository)
    {
        var (jwtToken, user) = await repository.LoginAsync(req);

        return TypedResults.Ok(new LoginRes
        {
            User = user,
            JwtToken = jwtToken
        });
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Login";

        return operation;
    }
}