using CoreShared;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;
using ProductService.Database.Entities;
using ProductService.Mappers;
using ProtobufSpec.Events;

namespace ProductService.Services;

public class ProductService(AppDbContext dbContext, IBus bus) : IProductService
{
    public async Task<ProductDto> CreateProductAsync(string name, string description, long priceCents, string activationCode)
    {
        var taken = await dbContext.Products
            .AsNoTracking()
            .AnyAsync(x => x.ActivationCode == activationCode);

        if (taken)
            throw new ProblemException(ExceptionMessages.ProductActivationCodeTaken, "Code already exists");
        
        var productEntity = new ProductEntity
        {
            Name = name,
            Description = description,
            PriceCents = priceCents,
            ActivationCode = activationCode
        };
        
        dbContext.Products.Add(productEntity);
        await dbContext.SaveChangesAsync();

        return productEntity.ToProductDto();
    }

    public async Task<ProductDto?> GetProductAsync(Guid id, CancellationToken ct)
    {
        var productEntity = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompletedOrderId == null && x.Id == id, ct);
        
        return productEntity?.ToProductDto();
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync(CancellationToken ct = default)
    {
        var productsEntities = await dbContext.Products
            .AsNoTracking()
            .Where(x => x.CompletedOrderId == null).ToListAsync(ct);
        
        return productsEntities.Select(x => x.ToProductDto());
    }

    public async Task MaskAsCompletedAsync(Guid productId, Guid orderId, Guid userId)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == productId);

        if (product is null)
            throw new Exception(ExceptionMessages.ProductLost);
        
        product.CompletedOrderId = orderId;
        
        await bus.Publish(new ProductOrderCompletedEvent
        {
            UserId = userId,
            ActivationCode = product.ActivationCode
        });
        
        await dbContext.SaveChangesAsync();
    }
}