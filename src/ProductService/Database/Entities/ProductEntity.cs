namespace ProductService.Database.Entities;

public class ProductEntity
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public required decimal Price { get; set; }
    
    public required Guid CategoryId { get; set; }
    public required ProductCategoryEntity Category { get; set; }
}
