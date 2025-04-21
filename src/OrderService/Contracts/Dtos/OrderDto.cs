namespace OrderService.Contracts.Dtos;

public class OrderDto
{
    public required Guid Id { get; set; }
    
    public required Guid ProductId { get; set; }
    
    public bool Paid { get; set; }
    
    public DateTime? PaymentExpirationDate { get; set; }
}