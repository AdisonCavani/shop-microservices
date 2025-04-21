using CoreShared;
using Microsoft.EntityFrameworkCore;
using OrderService.Contracts.Dtos;
using OrderService.Database;
using OrderService.Database.Entities;
using OrderService.Mappers;
using ProductService;

namespace OrderService.Services;

public class OrderService(AppDbContext dbContext, ProductAPI.ProductAPIClient client) : IOrderService
{
    public async Task<OrderDto?> CreateOrderAsync(Guid productId, Guid userId)
    {
        var orderExists = await dbContext.Orders.AnyAsync(x => x.ProductId == productId);

        if (orderExists)
            throw new ProblemException(ExceptionMessages.ProductSold, "This product is already sold");
        
        var response = await client.GetProductAsync(new GetProductReq
        {
            Id = productId.ToString()
        });

        if (response is null)
            return null;
        
        var orderEntity = new OrderEntity
        {
            UserId = userId,
            ProductId = productId
        };
        
        dbContext.Orders.Add(orderEntity);
        await dbContext.SaveChangesAsync();

        return orderEntity.ToOrderDto();
    }

    public async Task<OrderDto?> GetOrderAsync(Guid orderId, Guid userId)
    {
        var orderEntity = await dbContext.Orders
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == orderId);

        var payment = orderEntity?.Payments.FirstOrDefault(x => x.OrderId == orderId && (x.Paid || (!x.Paid && x.ExpiresAt > DateTime.UtcNow)));
        
        return orderEntity?.ToOrderDto(payment);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersAsync(Guid userId)
    {
        var orderEntities = await dbContext.Orders
            .Where(x => x.UserId == userId)
            .Include(x => x.Payments)
            .ToListAsync();
        
        var orders = orderEntities
            .Select(x => x.ToOrderDto(x.Payments
                .FirstOrDefault(y => y.Paid || (!y.Paid && y.ExpiresAt > DateTime.UtcNow))));

        return orders;
    }
}