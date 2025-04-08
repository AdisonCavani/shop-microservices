using Gateway.Mappers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using NotificationService;
using ProtobufSpec.Dtos;
using ProtobufSpec.Responses;
using ProductService;

namespace Gateway.Endpoints;

public static class Health
{
    public static async Task<Results<JsonHttpResult<HealthCheckRes[]>, Ok<HealthCheckRes[]>>> HandleAsync(
        [FromServices] HealthCheckService service,
        CancellationToken ct = default)
    {
        var gatewayReport = await service.CheckHealthAsync(ct);
        var gatewayResponse = new HealthCheckRes
        {
            ServiceName = AppDomain.CurrentDomain.FriendlyName,
            Status = gatewayReport.Status.ToString(),
            Checks = gatewayReport.Entries.Select(x => new HealthCheckDto
            {
                Component = x.Key,
                Status = x.Value.Status.ToString(),
                Description = x.Value.Description
            }),
            Duration = gatewayReport.TotalDuration
        };
        
        var notificationChannel = GrpcChannel.ForAddress("https://localhost:7204");
        var notificationClient = new NotificationAPI.NotificationAPIClient(notificationChannel);
        var notificationReport = await notificationClient.HealthAsync(new Empty());
        
        // TODO: retreive this from IOptions
        var productChannel = GrpcChannel.ForAddress("https://localhost:7100");
        var productClient = new ProductAPI.ProductAPIClient(productChannel);
        var productReport = await productClient.HealthAsync(new Empty());

        var response = new []
        {
            gatewayResponse,
            notificationReport.ToHealthCheckRes(),
            productReport.ToHealthCheckRes(),
        };

        var healthy = response.All(res => res.Status == HealthStatus.Healthy.ToString());
        
        return healthy ? TypedResults.Ok(response) : TypedResults.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
    
    public static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get health check report";

        return operation;
    }
}