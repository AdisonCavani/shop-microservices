using CoreShared;
using OrderService.Contracts.Requests;
using ProtobufSpec;

namespace OrderService.Endpoints;

public static class Mapper
{
    private static void MapOrderApi(this RouteGroupBuilder group)
    {
        group.MapPost("/", Order.Create.HandleAsync)
            .AddEndpointFilter<ValidationFilter<CreateOrderReq>>()
            .WithOpenApi(Order.Create.OpenApi);
        
        group.MapGet(ApiRoutes.Order.Get, Order.Get.HandleAsync)
            .WithOpenApi(Order.Get.OpenApi);
        
        group.MapGet("/", Order.List.HandleAsync)
            .WithOpenApi(Order.Get.OpenApi);

        group.WithTags("Order endpoints");
    }

    private static void MapPaymentApi(this RouteGroupBuilder group)
    {
        group.MapPost(ApiRoutes.Payment.Webhook, Payment.Webhook.HandleAsync)
            .WithOpenApi(Payment.Webhook.OpenApi);

        group.MapPost("/", Payment.Create.HandleAsync)
            .AddEndpointFilter<ValidationFilter<CreatePaymentReq>>()
            .WithOpenApi(Payment.Create.OpenApi);
        
        group.MapGet(ApiRoutes.Payment.Get, Payment.Get.HandleAsync)
            .WithOpenApi(Payment.Get.OpenApi);
        
        group.WithTags("Payment endpoints");
    }
    
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGrpcService<OrderGrpcService>();
        
        app.MapGroup(ApiRoutes.Order.Path).MapOrderApi();
        app.MapGroup(ApiRoutes.Payment.Path).MapPaymentApi();
    }
}