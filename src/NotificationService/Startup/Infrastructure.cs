using RabbitMQ.Client;

namespace NotificationService.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IConnection>(_ => new ConnectionFactory 
        { 
            HostName = "localhost", 
            Port = 5672
        }.CreateConnection());
        
        services.AddHealthChecks().AddRabbitMQ();
    }
}