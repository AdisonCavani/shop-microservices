using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Server.Contracts.Dtos;
using Server.Contracts.Responses;

namespace Server.Endpoints;

public static class Health
{
    internal static async Task<Results<StatusCodeHttpResult, Ok<HealthCheckRes>>> HandleAsync(
        [FromServices] HealthCheckService service,
        CancellationToken ct = default)
    {
        var report = await service.CheckHealthAsync(ct);
        var response = new HealthCheckRes
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(x => new HealthCheckDto
            {
                Component = x.Key,
                Status = x.Value.Status.ToString(),
                Description = x.Value.Description
            }),
            Duration = report.TotalDuration
        };

        return report.Status == HealthStatus.Healthy
            ? TypedResults.Ok(response)
            : TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable); // TODO: add response
    }
    
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get health check report";

        return operation;
    }
}