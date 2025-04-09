using Fluid;
using Microsoft.Extensions.FileProviders;
using NotificationService.Services;

namespace NotificationService.Startup;

public static class Services
{
    public static void AddServices(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(environment.ContentRootPath, "Templates")));
        services.AddSingleton<FluidParser>();

        services.AddScoped<EmailService>();
        services.AddScoped<EmailHandler>();
        
        services.AddHostedService<ConfirmEmailEventConsumer>();
    }
}