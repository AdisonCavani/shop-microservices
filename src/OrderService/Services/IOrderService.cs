using OrderService.Contracts.Dtos;

namespace OrderService.Services;

public interface IOrderService
{
    Task<OrderDto?> CreateOrderAsync(Guid productId, Guid userId);

    Task<OrderDto?> GetOrderAsync(Guid orderId, Guid userId);
    
    Task<IEnumerable<OrderDto>> GetOrdersAsync(Guid userId);
}