using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Gateway.Contracts;
using Gateway.Contracts.Dtos;
using Gateway.Contracts.Requests;
using Gateway.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Gateway.Endpoints.User;

public static class Register
{
    internal static async Task<Results<StatusCodeHttpResult, Created<UserDto>>> HandleAsync(
        [FromBody] RegisterReq req,
        HttpContext context,
        [FromServices] IUserRepository repository)
    {
        var (claims, authProperties, user) = await repository.RegisterAsync(req);

        await context.SignInAsync(
            new(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            authProperties);

        return TypedResults.Created($"{ApiRoutes.User.BasePath}/{user.Id}", user);
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Register endpoint";

        return operation;
    }
}