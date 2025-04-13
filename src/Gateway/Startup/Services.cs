using CoreShared.Transit;
using Gateway.Repositories;
using ProtobufSpec.Events;

namespace Gateway.Startup;

public static class Services
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<Publisher<ConfirmEmailEvent>>();
        services.AddHttpContextAccessor();
        services.AddScoped<JwtService>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}