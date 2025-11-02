using OrderService.Database;
using OrderService.Services;
using ProtobufSpec;

namespace OrderService.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(ServiceDefinitions.Order.Database);
        builder.AddMassTransitRabbitMq(ServiceDefinitions.RabbitMQ, _ => { }, configurator =>
        {
            configurator.AddConsumer<PaymentSucceededEventConsumer>();
        });
    }
}