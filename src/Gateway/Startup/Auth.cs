using Gateway.Auth;
using Gateway.Database.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using ProtobufSpec;

namespace Gateway.Startup;

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
            .AddCookie(options =>
            {
                options.LoginPath = ApiRoutes.User.Login;
                options.LogoutPath = ApiRoutes.User.Logout;
                
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("authentication-required", policy => policy.RequireAuthenticatedUser());
        });
    }
}