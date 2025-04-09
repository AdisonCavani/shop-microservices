using System.Diagnostics;
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

        var response = new []
        {
            gatewayResponse,
            await GetNotificationServiceHealthAsync(),
            await GetProductServiceHealthAsync(),
        };

        var healthy = response.All(res => res.Status == HealthStatus.Healthy.ToString());
        
        return healthy ? TypedResults.Ok(response) : TypedResults.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<HealthCheckRes> GetNotificationServiceHealthAsync()
    {
        var watch = Stopwatch.StartNew();

        try
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7204");
            var client = new NotificationAPI.NotificationAPIClient(channel);
            var report = await client.HealthAsync(new Empty());

            return report.ToHealthCheckRes();
        }
        catch (Exception ex)
        {
            watch.Stop();
            
            return new HealthCheckRes
            {
                ServiceName = nameof(NotificationService),
                Status = HealthStatus.Unhealthy.ToString(),
                Checks = new[]
                {
                    new HealthCheckDto
                    {
                        Component = "GRPC",
                        Description = ex.Message,
                        Status = HealthStatus.Unhealthy.ToString(),
                    }
                },
                Duration = watch.Elapsed
            };
        }
    }

    private static async Task<HealthCheckRes> GetProductServiceHealthAsync()
    {
        var watch = Stopwatch.StartNew();

        try
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7100");
            var client = new ProductAPI.ProductAPIClient(channel);
            var report = await client.HealthAsync(new Empty());

            return report.ToHealthCheckRes();
        }
        catch (Exception ex)
        {
            watch.Stop();
            
            return new HealthCheckRes
            {
                ServiceName = nameof(ProductService),
                Status = HealthStatus.Unhealthy.ToString(),
                Checks = new[]
                {
                    new HealthCheckDto
                    {
                        Component = "GRPC",
                        Description = ex.Message,
                        Status = HealthStatus.Unhealthy.ToString(),
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