using MassTransit;
using ProtobufSpec.Events;

namespace OrderService.Services;

public class PaymentSucceededEventConsumer(IPaymentService paymentService) : IConsumer<PaymentSucceededEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        await paymentService.PaymentReceivedAsync(context.Message);
    }
}