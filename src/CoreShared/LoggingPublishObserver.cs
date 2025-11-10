using MassTransit;
using Microsoft.Extensions.Logging;

namespace CoreShared;

public class LoggingPublishObserver(ILogger<LoggingPublishObserver> logger) : IPublishObserver
{
    public Task PrePublish<T>(PublishContext<T> context) where T : class
    {
        logger.LogInformation("Sending event {EventType} with MessageId {MessageId}", typeof(T).Name, context.MessageId);
        return Task.CompletedTask;
    }

    public Task PostPublish<T>(PublishContext<T> context) where T : class
    {
        logger.LogInformation("Event {EventType} sent successfully with MessageId {MessageId}", typeof(T).Name, context.MessageId);
        return Task.CompletedTask;
    }

    public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
    {
        logger.LogError(exception, "Error sending event {EventType} with MessageId {MessageId}", typeof(T).Name, context.MessageId);
        return Task.CompletedTask;
    }
}