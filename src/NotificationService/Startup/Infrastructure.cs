using NotificationService.Services;

namespace NotificationService.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddMassTransitRabbitMq("rabbitmq", _ => {}, configurator =>
        {
            configurator.AddConsumer<ConfirmEmailEventConsumer>();
            configurator.AddConsumer<OrderCompletedEmailEventConsumer>();
        });
    }
}