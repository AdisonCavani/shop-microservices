using CoreShared;
using Gateway.Database.Entities;
using Microsoft.AspNetCore.Identity;

namespace Gateway.Startup;

public static class Auth
{
    public static void AddAuth(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
        services.AddJwtBearerAuth(appSettings);
        services.AddAuthorization(options =>
        {
            options.AddPolicy("authentication-required", policy => policy.RequireAuthenticatedUser());
            options.AddPolicy("admin-required", policy => policy.RequireRole(UserRoleEnum.Admin.ToString()));
        });
    }
}