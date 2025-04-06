using Microsoft.EntityFrameworkCore;
using ProductService.Database;

namespace ProductService.Startup;

public static class Infrastucture
{
    public static void AddInfrastructure(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseNpgsql(appSettings.PostgresConnectionString,
                npgSettings => npgSettings.EnableRetryOnFailure());
        });

        services.AddHealthChecks().AddNpgSql(appSettings.PostgresConnectionString);
    }
}