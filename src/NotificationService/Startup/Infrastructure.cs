using MassTransit;
using NotificationService.Database;
using NotificationService.Services;
using ProtobufSpec.Events;

namespace NotificationService.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>("Notifications");
        builder.AddMassTransitRabbitMq("rabbitmq", _ => {}, configurator =>
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