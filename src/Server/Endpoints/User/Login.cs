using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Server.Contracts.Dtos;
using Server.Contracts.Requests;
using Server.Repositories;

namespace Server.Endpoints.User;

public static class Login
{
    internal static async Task<Results<StatusCodeHttpResult, Ok<UserDto>>> HandleAsync(
        [FromBody] LoginReq req,
        HttpContext context,
        [FromServices] IUserRepository repository)
    {
        var (claims, authProperties, user) = await repository.LoginAsync(req);

        await context.SignInAsync(
            new(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            authProperties);

        return TypedResults.Ok(user);
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Login endpoint";

        return operation;
    }
}