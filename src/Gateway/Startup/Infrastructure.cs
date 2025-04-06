using CoreShared.Settings;
using Gateway.Database;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Gateway.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddSingleton<IConnection>(_ => new ConnectionFactory 
        { 
            HostName = "localhost", 
            Port = 5672
        }.CreateConnection());
        
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(appSettings.RedisConnectionString));
        
        services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseNpgsql(appSettings.PostgresConnectionString,
                npgSettings => npgSettings.EnableRetryOnFailure());
        });
        
        services.AddHealthChecks()
            .AddNpgSql(appSettings.PostgresConnectionString)
            .AddRedis(appSettings.RedisConnectionString)
            .AddRabbitMQ();
    }
}