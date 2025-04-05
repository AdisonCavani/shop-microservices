using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Server.Auth;
using Server.Database.Entities;

namespace Server.Startup;

public static class Auth
{
    public static void AddAuth(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
        services.AddSingleton<ITicketStore, RedisTicketStore>();
        services
            .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
            .Configure<ITicketStore>((options, store) =>
            {
                options.SessionStore = store;
                options.Cookie.Name = AuthSchema.CookieName;
            });
        
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("authentication-required", policy => policy.RequireAuthenticatedUser());
        });
    }
}