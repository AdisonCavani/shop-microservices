using Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ProductService.Services;

public class ProductGrpcService(HealthCheckService service, [FromServices] IProductService productService) : ProductAPI.ProductAPIBase
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

    public override async Task<ProductDto?> GetProduct(GetProductReq request, ServerCallContext context)
    {
        return await productService.GetProductAsync(Guid.Parse(request.Id));
    }
}