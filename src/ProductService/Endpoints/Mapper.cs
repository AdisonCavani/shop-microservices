using CoreShared;
using ProductService.Contracts.Requests;
using ProtobufSpec;

namespace ProductService.Endpoints;

public static class Mapper
{
    private static void MapProductApi(this RouteGroupBuilder group)
    {
        group.MapPost("/", Product.Create.HandleAsync)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<CreateProductReq>>()
            .WithOpenApi(Product.Create.OpenApi);

        group.MapGet(ApiRoutes.Product.Get, Product.Get.HandleAsync)
            .WithOpenApi(Product.Get.OpenApi);
        
        group.MapGet("/", Product.List.HandleAsync)
            .WithOpenApi(Product.List.OpenApi);

        group.WithTags("Product endpoints");
    }
    
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGrpcService<ProductGrpcService>();
        
        app.MapGroup(ApiRoutes.Product.Path).MapProductApi();
    }
}