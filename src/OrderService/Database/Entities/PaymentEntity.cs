namespace OrderService.Database.Entities;

public class PaymentEntity
{
    public required Guid Id { get; set; }
    
    public required string StripeCheckoutId { get; set; }
    
    public required DateTime CreatedAt { get; set; }
    
    public required DateTime ExpiresAt { get; set; }
    
    public bool Paid { get; set; }
    
    public required Guid OrderId { get; set; }
    public OrderEntity Order { get; set; } = default!;
}
