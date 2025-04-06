using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Gateway.Contracts.Dtos;
using Gateway.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Gateway.Endpoints.User;

public static class Get
{
    internal static async Task<Results<StatusCodeHttpResult, Ok<UserDto>>> HandleAsync(
        HttpContext context,
        [FromServices] IUserRepository repository)
    {
        var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new Exception(ExceptionMessages.NameIdentifierNull);

        var userId = Guid.Parse(userIdStr);
        var user = await repository.GetUserByIdAsync(userId);

        return TypedResults.Ok(user);
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get user endpoint";

        return operation;
    }
}