using CoreShared.Transit;
using ProtobufSpec.Events;
using RabbitMQ.Client;

namespace NotificationService.Services;

public class ConfirmEmailEventConsumer : Consumer<ConfirmEmailEvent>
{
    public ConfirmEmailEventConsumer(IServiceProvider serviceProvider, ILogger<Consumer<ConfirmEmailEvent>> logger, IConnection connection) : base(serviceProvider, logger, connection)
    {
    }
    
    protected override async Task Consume(ConfirmEmailEvent message, IServiceProvider serviceProvider, CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var emailHandler = scope.ServiceProvider.GetRequiredService<EmailHandler>();
            
        await emailHandler.VerificationMailAsync(message, ct);
    }
}