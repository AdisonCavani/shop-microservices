using Server.Repositories;
using Server.Services;

namespace Server.Startup;

public static class Services
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddSingleton<MessageBusPublisher>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}