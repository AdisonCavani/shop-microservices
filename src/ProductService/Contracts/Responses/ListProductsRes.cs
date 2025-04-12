namespace ProductService.Contracts.Responses;

public class ListProductsRes
{
    public IEnumerable<ProductDto> Products { get; set; } = Enumerable.Empty<ProductDto>();
}