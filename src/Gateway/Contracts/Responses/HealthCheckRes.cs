using Gateway.Contracts.Dtos;

namespace Gateway.Contracts.Responses;

public class HealthCheckRes
{
    public required string Status { get; set; }

    public IEnumerable<HealthCheckDto> Checks { get; set; } = Enumerable.Empty<HealthCheckDto>();

    public TimeSpan Duration { get; set; }
}