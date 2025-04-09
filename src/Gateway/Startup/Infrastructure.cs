using Gateway.Database;

namespace Gateway.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>("Users");
        builder.AddRabbitMQClient("rabbitmq");
        builder.AddRedisClient("redis");
    }
}