using CoreShared;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;
using ProtobufSpec.Events;

namespace ProductService.Services;

public class OrderCompletedEventConsumer(IBus bus, AppDbContext dbContext) : IConsumer<OrderCompletedEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var productId = context.Message.ProductId;
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);

        if (product is null)
            throw new Exception(ExceptionMessages.ProductLost);
        
        product.CompletedOrderId = context.Message.OrderId;
        await dbContext.SaveChangesAsync();

        await bus.Publish(new OrderCompletedEmailEvent
        {
            ActivationCode = product.ActivationCode,
            UserId = context.Message.UserId
        });
    }
}