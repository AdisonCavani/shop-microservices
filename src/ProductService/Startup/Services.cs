using ProductService.Services;

namespace ProductService.Startup;

public static class Services
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService.Services.ProductService>();
    }
}