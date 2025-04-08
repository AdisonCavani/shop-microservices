using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ProductService.Endpoints;

public class ProductService(HealthCheckService service) : ProductAPI.ProductAPIBase
{
    public override async Task<HealthResponse> Health(Empty request, ServerCallContext context)
    {
        var report = await service.CheckHealthAsync();
        
        var response = new HealthResponse
        {
            ServiceName = nameof(ProductService),
            Status = report.Status.ToString(),
            Duration = report.TotalDuration.ToString()
        };
        
        response.Checks.AddRange(report.Entries.Select(x => new HealthCheck
        {
            Component = x.Key,
            Status = x.Value.Status.ToString(),
            Description = x.Value.Description
        }));
        
        return response;
    }
}