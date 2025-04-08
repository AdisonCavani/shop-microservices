namespace ProductService.Contracts.Dtos;

public class CategoryDto
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
}