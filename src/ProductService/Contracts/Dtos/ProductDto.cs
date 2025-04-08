namespace ProductService.Contracts.Dtos;

public class ProductDto
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public required decimal Price { get; set; }
    
    public required CategoryDto Category { get; set; }
}