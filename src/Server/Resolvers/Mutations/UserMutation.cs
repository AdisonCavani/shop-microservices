using System.Security.Claims;
using AppAny.HotChocolate.FluentValidation;
using AutoMapper;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Server.Database;
using Server.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Contracts.Dtos;
using Server.Contracts.Requests;
using Server.Validators;

namespace Server.Resolvers.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutation
{
    public async Task<UserDto> Register(
        [Service] IMapper mapper,
        [Service] IPasswordHasher<UserEntity> hasher,
        [Service] AppDbContext context,
        [Service] IHttpContextAccessor accessor,
        [UseFluentValidation, UseValidator<RegisterRequestValidator>] RegisterRequest req)
    {
        var user = new UserEntity
        {
            Email = req.Email.ToLower()
        };

        var hashedPassword = hasher.HashPassword(user, req.Password);
        user.Password = hashedPassword;

        context.Users.Add(user);
        var result = await context.SaveChangesAsync();

        if (result <= 0)
            throw new GraphQLException(ExceptionMessages.DatabaseProblem);

        if (accessor.HttpContext is null)
            throw new GraphQLException(ExceptionMessages.HttpContextNull);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow.AddYears(2),
            IsPersistent = true
        };

        await accessor.HttpContext.SignInAsync(
            new(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            authProperties);

        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> Login(
        [Service] IMapper mapper,
        [Service] AppDbContext context,
        [Service] IPasswordHasher<UserEntity> hasher,
        [Service] IHttpContextAccessor accessor,
        [UseFluentValidation, UseValidator<LoginRequestValidator>] LoginRequest req)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == req.Email.ToLower());

        if (user is null)
            throw new GraphQLException(ExceptionMessages.EmailNotExist);

        var valid = hasher.VerifyHashedPassword(user, user.Password, req.Password);

        if (valid == PasswordVerificationResult.Failed)
            throw new GraphQLException(ExceptionMessages.PasswordInvalid);

        if (valid == PasswordVerificationResult.SuccessRehashNeeded)
            return null; // TODO: fix

        if (accessor.HttpContext is null)
            throw new GraphQLException(ExceptionMessages.HttpContextNull);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow.AddYears(2),
            IsPersistent = req.Persistent
        };

        await accessor.HttpContext.SignInAsync(
            new(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            authProperties);

        return mapper.Map<UserDto>(user);
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