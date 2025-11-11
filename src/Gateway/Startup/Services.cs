using Gateway.Repositories;

namespace Gateway.Startup;

public static class Services
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<JwtService>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}