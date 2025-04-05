using Server.Contracts;
using Server.Contracts.Requests;
using Server.Startup;
using Server.Validators;

namespace Server.Endpoints;

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
        
        group.MapPost(ApiRoutes.User.VerifyEmail, User.VerifyEmail.HandleAsync)
            .AddEndpointFilter<ValidationFilter<VerifyEmailReq>>()
            .WithOpenApi(User.VerifyEmail.OpenApi);
        
        group.MapPost(ApiRoutes.User.Logout, User.Logout.HandleAsync)
            .RequireAuthorization()
            .WithOpenApi(User.Logout.OpenApi);

        group.MapGet("/", User.Get.HandleAsync)
            .RequireAuthorization()
            .WithOpenApi(User.Get.OpenApi);

        group.WithTags("User endpoint");
    }
    
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet(ApiRoutes.Health, Health.HandleAsync)
            .WithTags("Health Endpoint")
            .WithOpenApi(Health.OpenApi);
        
        app.MapGroup(ApiRoutes.User.BasePath).MapUserApi();

        app.MapGetSwaggerForYarp(app.Configuration);
    }
}