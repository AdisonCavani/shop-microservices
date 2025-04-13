using System.Diagnostics.CodeAnalysis;
using Gateway.Contracts.Dtos;
using Gateway.Contracts.Requests;
using Gateway.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProtobufSpec;

namespace Gateway.Endpoints.User;

public static class Register
{
    internal static async Task<Results<StatusCodeHttpResult, Created<UserDto>>> HandleAsync(
        [FromBody] RegisterReq req,
        HttpContext context,
        [FromServices] IUserRepository repository)
    {
        var user = await repository.RegisterAsync(req);

        return TypedResults.Created(ApiRoutes.User.Path, user);
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Create new user";

        return operation;
    }
}