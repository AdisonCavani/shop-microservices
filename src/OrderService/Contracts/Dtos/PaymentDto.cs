namespace OrderService.Contracts.Dtos;

public class PaymentDto
{
    public string? PaymentUrl { get; set; }
    
    public bool Paid { get; set; }
    
    public DateTime? ExpirationDate { get; set; }
}