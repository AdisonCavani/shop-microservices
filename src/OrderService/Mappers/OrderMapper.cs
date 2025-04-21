using OrderService.Contracts.Dtos;
using OrderService.Database.Entities;

namespace OrderService.Mappers;

public static class OrderMapper
{
    public static OrderDto ToOrderDto(this OrderEntity orderEntity, PaymentEntity? paymentEntity = null)
    {
        return new OrderDto
        {
            Id = orderEntity.Id,
            ProductId = orderEntity.ProductId,
            Paid = paymentEntity?.Paid ?? false,
            PaymentId = paymentEntity?.Id,
            PaymentExpirationDate = paymentEntity?.Paid ?? true ? null : paymentEntity.ExpiresAt 
        };
    }
}