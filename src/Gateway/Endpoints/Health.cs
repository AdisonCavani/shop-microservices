using System.Diagnostics;
using Common;
using Gateway.Mappers;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using NotificationService;
using OrderService;
using ProtobufSpec.Dtos;
using ProtobufSpec.Responses;
using ProductService;
using ProtobufSpec;

namespace Gateway.Endpoints;

public static class Health
{
    public static async Task<Results<JsonHttpResult<HealthCheckRes[]>, Ok<HealthCheckRes[]>>> HandleAsync(
        [FromServices] HealthCheckService service,
        [FromServices] NotificationAPI.NotificationAPIClient notificationClient,
        [FromServices] OrderAPI.OrderAPIClient orderClient,
        [FromServices] ProductAPI.ProductAPIClient productClient,
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

        var response = new []
        {
            gatewayResponse,
            await GetGrpcServiceHealthAsync(notificationClient, ServiceDefinitions.Notification.Name, async method => await method.HealthAsync(new Empty())),
            await GetGrpcServiceHealthAsync(orderClient, ServiceDefinitions.Order.Name, async method => await method.HealthAsync(new Empty())),
            await GetGrpcServiceHealthAsync(productClient, ServiceDefinitions.Product.Name, async method => await method.HealthAsync(new Empty()))
        };

        var healthy = response.All(res => res.Status == nameof(HealthStatus.Healthy));
        
        return healthy ? TypedResults.Ok(response) : TypedResults.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<HealthCheckRes> GetGrpcServiceHealthAsync<TGrpcClient>(
        TGrpcClient client,
        string serviceName,
        Func<TGrpcClient, Task<HealthResponse>> rpcFunctionAsync)
    {
        var watch = Stopwatch.StartNew();

        try
        {
            var report = await rpcFunctionAsync(client);
            return report.ToHealthCheckRes();
        }
        catch (Exception ex)
        {
            watch.Stop();
            
            return new HealthCheckRes
            {
                ServiceName = serviceName,
                Status = nameof(HealthStatus.Unhealthy),
                Checks = new[]
                {
                    new HealthCheckDto
                    {
                        Component = "GRPC",
                        Description = ex.Message,
                        Status = nameof(HealthStatus.Unhealthy)
                    }
                },
                Duration = watch.Elapsed
            };
        }
    }
    
    public static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get health check report";

        return operation;
    }
}