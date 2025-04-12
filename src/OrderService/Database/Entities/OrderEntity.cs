namespace OrderService.Database.Entities;

public class OrderEntity
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public required Guid UserId { get; set; }
    
    public required Guid ProductId { get; set; }
    
    public required int Quantity { get; set; }
    
    public List<PaymentEntity> Payments { get; set; } = [];
}
