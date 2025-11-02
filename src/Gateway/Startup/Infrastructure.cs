using Gateway.Database;
using Microsoft.EntityFrameworkCore;
using ProtobufSpec;

namespace Gateway.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(ServiceDefinitions.Identity.Database, configureDbContextOptions: (options) =>
            options.UseNpgsql(npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<UserRoleEnum>("UserRoleEnum");
            }));
        builder.AddRedisClient(ServiceDefinitions.Redis);
        builder.AddMassTransitRabbitMq(ServiceDefinitions.RabbitMQ);
    }
}