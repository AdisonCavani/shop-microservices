namespace ProductService.Services;

public interface IProductService
{
    Task<ProductDto> CreateProductAsync(string name, string description, long priceCents, string activationCode);
    
    Task<ProductDto?> GetProductAsync(Guid id, CancellationToken ct = default);
    
    Task<IEnumerable<ProductDto>> GetProductsAsync(CancellationToken ct = default);
    
    Task MaskAsCompletedAsync(Guid productId, Guid orderId, Guid userId);
}