using MassTransit;
using ProtobufSpec.Events;

namespace NotificationService.Services;

public class OrderCompletedEmailEventConsumer(EmailHandler emailHandler) : IConsumer<OrderCompletedEmailEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEmailEvent> context)
    {
        await emailHandler.OrderCompletedMailAsync(context.Message);
    }
}