using ProductService.Database;
using ProductService.Services;

namespace ProductService.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>("Products");
        builder.AddMassTransitRabbitMq("rabbitmq", _ => {}, configurator =>
        {
            configurator.AddConsumer<OrderCompletedEventConsumer>();
        });
    }
}