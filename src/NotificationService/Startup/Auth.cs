using CoreShared;

namespace NotificationService.Startup;

public static class Auth
{
    public static void AddAuth(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddJwtBearerAuth(appSettings);
        services.AddAuthorization();
    }
}