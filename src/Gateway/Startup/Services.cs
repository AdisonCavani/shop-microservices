using CoreShared.Transit;
using Gateway.Repositories;
using ProtobufSpec.Events;

namespace Gateway.Startup;

public static class Services
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddSingleton<Publisher<ConfirmEmailEvent>>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}