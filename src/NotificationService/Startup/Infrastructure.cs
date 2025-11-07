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
        builder.Services.AddMassTransit(configurator =>
        {
            configurator.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromMinutes(10);
                o.QueryDelay = TimeSpan.FromSeconds(3);
                o.UsePostgres().UseBusOutbox();
            });
            
            configurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration.GetConnectionString(ServiceDefinitions.RabbitMQ));
                cfg.ConfigureEndpoints(context);
            });
            
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