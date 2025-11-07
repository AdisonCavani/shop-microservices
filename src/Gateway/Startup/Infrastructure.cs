using Gateway.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProtobufSpec;

namespace Gateway.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(ServiceDefinitions.Identity.Database, configureDbContextOptions: (options) =>
            options.UseNpgsql(npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<UserRoleEnum>("UserRoleEnum");
            }));
        builder.AddRedisClient(ServiceDefinitions.Redis);
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
        });
    }
}