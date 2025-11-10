using MassTransit;
using Microsoft.Extensions.Logging;

namespace CoreShared;

public class LoggingConsumeObserver(ILogger<LoggingConsumeObserver> logger) : IConsumeObserver
{
    public Task PreConsume<T>(ConsumeContext<T> context) where T : class
    {
        logger.LogInformation("Receiving event {EventType} with MessageId {MessageId}", typeof(T).Name, context.MessageId);
        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context) where T : class
    {
        logger.LogInformation("Successfully consumed {EventType} with MessageId {MessageId}", typeof(T).Name, context.MessageId);
        return Task.CompletedTask;
    }

    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        logger.LogError(exception, "Error consuming {EventType} with MessageId {MessageId}", typeof(T).Name, context.MessageId);
        return Task.CompletedTask;
    }
}