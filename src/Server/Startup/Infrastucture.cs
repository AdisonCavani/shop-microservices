using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Server.Database;
using StackExchange.Redis;

namespace Server.Startup;

public static class Infrastucture
{
    public static void AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        var dbSettings = configuration.GetSection(nameof(DbSettings)).Get<DbSettings>()!.Validate();

        services.AddSingleton<IConnection>(_ => new ConnectionFactory 
        { 
            HostName = "localhost", 
            Port = 5672
        }.CreateConnection());
        
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(dbSettings.RedisConnectionString));
        
        services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseNpgsql(dbSettings.PostgresConnectionString,
                npgSettings => npgSettings.EnableRetryOnFailure());
        });
        
        services.AddHealthChecks()
            .AddNpgSql(dbSettings.PostgresConnectionString)
            .AddRedis(dbSettings.RedisConnectionString)
            .AddRabbitMQ();
    }
}