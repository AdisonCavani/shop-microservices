using Fluid;
using NotificationService.Services;

namespace NotificationService.Startup;

public static class Services
{
    public static void AddServices(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddSingleton<FluidParser>();
        services.AddScoped<EmailService>();
        services.AddScoped<INotificationService, NotificationService.Services.NotificationService>();
    }
}