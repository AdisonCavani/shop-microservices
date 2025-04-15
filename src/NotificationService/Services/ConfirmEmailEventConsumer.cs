using MassTransit;
using ProtobufSpec.Events;

namespace NotificationService.Services;

public class ConfirmEmailEventConsumer(EmailHandler emailHandler) : IConsumer<ConfirmEmailEvent>
{

    public async Task Consume(ConsumeContext<ConfirmEmailEvent> context)
    {
        await emailHandler.VerificationMailAsync(context.Message);
    }
}