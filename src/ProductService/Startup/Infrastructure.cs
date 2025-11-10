using CoreShared;
using MassTransit;
using ProductService.Database;
using ProductService.Services;
using ProtobufSpec;

namespace ProductService.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(ServiceDefinitions.Product.Database);
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
                
                var consumeObserver = new LoggingConsumeObserver(context.GetRequiredService<ILogger<LoggingConsumeObserver>>());
                var publishObserver = new LoggingPublishObserver(context.GetRequiredService<ILogger<LoggingPublishObserver>>());

                cfg.ConnectConsumeObserver(consumeObserver);
                cfg.ConnectPublishObserver(publishObserver);
            });
            
            configurator.AddConsumer<OrderCompletedEventConsumer>();
        });
    }
}