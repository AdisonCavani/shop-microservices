using Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OrderService.Endpoints;

public class OrderGrpcService(HealthCheckService service) : OrderAPI.OrderAPIBase
{
    public override async Task<HealthResponse> Health(Empty request, ServerCallContext context)
    {
        var report = await service.CheckHealthAsync();
        
        var response = new HealthResponse
        {
            ServiceName = AppDomain.CurrentDomain.FriendlyName,
            Status = report.Status.ToString(),
            Duration = report.TotalDuration.ToString()
        };
        
        response.Checks.AddRange(report.Entries.Select(x => new HealthCheck
        {
            Component = x.Key,
            Status = x.Value.Status.ToString(),
            Description = x.Value.Description ?? string.Empty
        }));
        
        return response;
    }
}