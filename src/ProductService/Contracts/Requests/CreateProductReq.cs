namespace ProductService.Contracts.Requests;

public class CreateProductReq
{
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public required decimal Price { get; set; }
    
    public required Guid CategoryId { get; set; }
}