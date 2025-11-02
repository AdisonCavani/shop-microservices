using MassTransit;
using NotificationService.Database;
using NotificationService.Services;
using ProtobufSpec;
using ProtobufSpec.Events;

namespace NotificationService.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(ServiceDefinitions.Notification.Database);
        builder.AddMassTransitRabbitMq(ServiceDefinitions.RabbitMQ, _ => {}, configurator =>
        {
            var eventTypes = typeof(DomainEvent).Assembly
                .GetTypes()
                .Where(t => typeof(DomainEvent).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var eventType in eventTypes)
            {
                var consumerType = typeof(DomainEventConsumer<>).MakeGenericType(eventType);
                configurator.AddConsumer(consumerType);
            }
        });
    }
}