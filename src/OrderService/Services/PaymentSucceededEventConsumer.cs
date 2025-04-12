using CoreShared.Transit;
using Microsoft.EntityFrameworkCore;
using OrderService.Database;
using ProtobufSpec.Events;
using RabbitMQ.Client;

namespace OrderService.Services;

public class PaymentSucceededEventConsumer : Consumer<PaymentSucceededEvent>
{
    public PaymentSucceededEventConsumer(
        IConnection connection,
        IServiceProvider serviceProvider,
        ILogger<Consumer<PaymentSucceededEvent>> logger) : base(connection, serviceProvider, logger)
    {
    }

    protected override async Task Consume(PaymentSucceededEvent message, IServiceProvider serviceProvider, CancellationToken ct)
    {
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        var payment = await dbContext.Payments.FirstOrDefaultAsync(x => x.Id == message.PaymentId, ct);

        if (payment is null)
            throw new Exception("Could not find payment");

        payment.Paid = true;
        await dbContext.SaveChangesAsync(ct);
    }
}