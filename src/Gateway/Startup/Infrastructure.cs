using Gateway.Database;
using Gateway.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>("Users", configureDbContextOptions: (options) =>
            options.UseNpgsql(npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<UserRoleEnum>("UserRoleEnum");
            }));
        builder.AddRabbitMQClient("rabbitmq");
        builder.AddRedisClient("redis");
    }
}