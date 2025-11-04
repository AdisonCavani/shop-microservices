using MassTransit;
using ProtobufSpec.Events;

namespace ProductService.Services;

public class OrderCompletedEventConsumer(IProductService productService) : IConsumer<OrderCompletedEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        await productService.MaskAsCompletedAsync(context.Message.ProductId, context.Message.OrderId, context.Message.UserId!.Value);
    }
}