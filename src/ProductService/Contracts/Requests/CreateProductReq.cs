namespace ProductService.Contracts.Requests;

public class CreateProductReq
{
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public required long PriceCents { get; set; }
    
    public required string ActivationCode { get; set; }
}