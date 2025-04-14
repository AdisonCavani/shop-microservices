using CoreShared;
using Gateway.Contracts.Requests;
using Gateway.Startup;
using ProtobufSpec;

namespace Gateway.Endpoints;

public static class Mapper
{
    private static void MapUserApi(this RouteGroupBuilder group)
    {
        group.MapPost(ApiRoutes.User.Register, User.Register.HandleAsync)
            .AddEndpointFilter<ValidationFilter<RegisterReq>>()
            .WithOpenApi(User.Register.OpenApi);
        
        group.MapPost(ApiRoutes.User.Login, User.Login.HandleAsync)
            .AddEndpointFilter<ValidationFilter<LoginReq>>()
            .WithOpenApi(User.Login.OpenApi);
        
        group.MapGet(ApiRoutes.User.VerifyEmail, User.VerifyEmail.HandleAsync)
            .WithOpenApi(User.VerifyEmail.OpenApi);

        group.MapGet("/", User.Get.HandleAsync)
            .RequireAuthorization()
            .WithOpenApi(User.Get.OpenApi);

        group.WithTags("User endpoints");
    }
    
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet(ApiRoutes.Health, Health.HandleAsync)
            .WithTags("Health endpoint")
            .WithOpenApi(Health.OpenApi);
        
        app.MapGroup(ApiRoutes.User.Path).MapUserApi();

        app.MapGetSwaggerForYarp();
    }
}