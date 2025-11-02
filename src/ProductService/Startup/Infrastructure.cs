using ProductService.Database;
using ProductService.Services;
using ProtobufSpec;

namespace ProductService.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(ServiceDefinitions.Product.Database);
        builder.AddMassTransitRabbitMq(ServiceDefinitions.RabbitMQ, _ => {}, configurator =>
        {
            configurator.AddConsumer<OrderCompletedEventConsumer>();
        });
    }
}