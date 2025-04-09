namespace NotificationService.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddRabbitMQClient("rabbitmq");
    }
}