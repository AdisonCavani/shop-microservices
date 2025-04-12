using ProductService.Contracts.Requests;
using ProductService.Database.Entities;

namespace ProductService.Mappers;

public static class ProductMapper
{
    public static ProductDto ToProductDto(this ProductEntity productEntity)
    {
        return new ProductDto
        {
            Id = productEntity.Id.ToString(),
            Name = productEntity.Name,
            Description = productEntity.Description,
            PriceCents = productEntity.PriceCents
        };
    }

    public static ProductEntity ToProductEntity(this CreateProductReq req)
    {
        return new ProductEntity
        {
            Name = req.Name,
            Description = req.Description,
            PriceCents = req.PriceCents,
            ActivationCode = req.ActivationCode
        };
    }
}