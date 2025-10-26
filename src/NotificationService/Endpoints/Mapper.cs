using CoreShared;
using NotificationService.Contracts.Requests;
using NotificationService.Services;
using ProtobufSpec;

namespace NotificationService.Endpoints;

public static class Mapper
{
    private static void MapNotificationApi(this RouteGroupBuilder group)
    {
        group.MapPost("/", Notification.Create.HandleAsync)
            .AddEndpointFilter<ValidationFilter<CreateNotificationTriggerReq>>()
            .WithOpenApi(Notification.Create.OpenApi);
        
        group.MapGet(ApiRoutes.Notification.ById, Notification.Get.HandleAsync)
            .WithOpenApi(Notification.Get.OpenApi);
        
        group.MapGet("/", Notification.List.HandleAsync)
            .WithOpenApi(Notification.List.OpenApi);

        group.MapPost(ApiRoutes.Notification.ById, Notification.Update.HandleAsync)
            .WithOpenApi(Notification.Update.OpenApi);

        group.MapDelete(ApiRoutes.Notification.ById, Notification.Delete.HandleAsync)
            .WithOpenApi(Notification.Delete.OpenApi);
        
        group.WithTags("Notification endpoints");
    }
    
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGrpcService<NotificationGrpcService>();
        
        app.MapGroup(ApiRoutes.Notification.Path).MapNotificationApi();
    }
}