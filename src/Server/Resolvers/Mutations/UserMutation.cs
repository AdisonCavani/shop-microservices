using System.Security.Claims;
using AppAny.HotChocolate.FluentValidation;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Server.Contracts.Dtos;
using Server.Contracts.Requests;
using Server.Repositories;
using Server.Validators;

namespace Server.Resolvers.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutation
{
    public async Task<UserDto> Register(
        [Service] IUserRepository repository,
        [Service] IHttpContextAccessor accessor,
        [UseFluentValidation, UseValidator<RegisterRequestValidator>] RegisterRequest req)
    {
        var (claims, authProperties, user) = await repository.RegisterAsync(req);

        if (accessor.HttpContext is null)
            throw new GraphQLException(ExceptionMessages.HttpContextNull);

        await accessor.HttpContext.SignInAsync(
            new(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            authProperties);

        return user;
    }

    public async Task<UserDto?> Login(
        [Service] IUserRepository repository,
        [Service] IHttpContextAccessor accessor,
        [UseFluentValidation, UseValidator<LoginRequestValidator>] LoginRequest req)
    {
        var (claims, authProperties, user) = await repository.LoginAsync(req);

        if (accessor.HttpContext is null)
            throw new GraphQLException(ExceptionMessages.HttpContextNull);

        await accessor.HttpContext.SignInAsync(
            new(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            authProperties);

        return user;
    }

    public async Task<bool> VerifyEmail(
        [Service] IUserRepository repository,
        [UseFluentValidation, UseValidator<VerifyEmailRequestValidator>] VerifyEmailRequest req)
    {
        await repository.VerifyEmailAsync(req);
        return true;
    }

    [Authorize]
    public async Task<bool> Logout([Service] IHttpContextAccessor accessor)
    {
        if (accessor.HttpContext is null)
            throw new GraphQLException(ExceptionMessages.HttpContextNull);

        await accessor.HttpContext.SignOutAsync();
        return true;
    }
}