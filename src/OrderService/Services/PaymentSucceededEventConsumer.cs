using CoreShared;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Database;
using ProtobufSpec.Events;

namespace OrderService.Services;

public class PaymentSucceededEventConsumer(AppDbContext dbContext) : IConsumer<PaymentSucceededEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var payment = await dbContext.Payments.FirstOrDefaultAsync(x => x.Id == context.Message.PaymentId);

        if (payment is null)
            throw new Exception(ExceptionMessages.PaymentLost);

        payment.Paid = true;
        await dbContext.SaveChangesAsync();
    }
}