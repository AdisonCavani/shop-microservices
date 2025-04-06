namespace ProductService.Database.Entities;

public class ProductCategoryEntity
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }

    public IEnumerable<ProductEntity> Products { get; set; } = Enumerable.Empty<ProductEntity>();
}