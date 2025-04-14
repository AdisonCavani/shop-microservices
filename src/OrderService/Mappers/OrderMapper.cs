using OrderService.Contracts.Dtos;
using OrderService.Database.Entities;

namespace OrderService.Mappers;

public static class OrderMapper
{
    public static OrderDto ToOrderDto(this OrderEntity orderEntity)
    {
        return new OrderDto
        {
            Id = orderEntity.Id,
            ProductId = orderEntity.ProductId,
            Quantity = orderEntity.Quantity
        };
    }
}