using MassTransit;
using ProtobufSpec.Events;

namespace NotificationService.Services;

public class DomainEventConsumer<TDomainEvent>(INotificationService notificationService) : IConsumer<TDomainEvent> where TDomainEvent : DomainEvent
{
    public async Task Consume(ConsumeContext<TDomainEvent> context)
    {
        await notificationService.HandleDomainEventAsync(typeof(TDomainEvent).Name, context.Message);
    }
}