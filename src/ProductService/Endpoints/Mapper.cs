using CoreShared;
using ProductService.Contracts.Requests;
using ProtobufSpec;

namespace ProductService.Endpoints;

public static class Mapper
{
    private static void MapProductApi(this RouteGroupBuilder group)
    {
        group.MapPost("/", Product.Create.HandleAsync)
            .AddEndpointFilter<ValidationFilter<CreateProductReq>>()
            .WithOpenApi(Product.Create.OpenApi);

        group.WithTags("Product endpoint");
    }
    
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGrpcService<ProductService>();
        
        app.MapGroup(ApiRoutes.Product.BasePath).MapProductApi();
    }
}