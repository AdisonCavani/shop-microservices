namespace ProductService.Database.Entities;

public class ProductEntity
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public required long PriceCents { get; set; }
    
    public required string ActivationCode { get; set; }
    
    public Guid? CompletedOrderId { get; set; }
}
