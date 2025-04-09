using ProtobufSpec.Dtos;

namespace ProtobufSpec.Responses;

public class HealthCheckRes
{
    public required string ServiceName { get; set; }
    
    public required string Status { get; set; }

    public IEnumerable<HealthCheckDto> Checks { get; set; } = Enumerable.Empty<HealthCheckDto>();

    public TimeSpan Duration { get; set; }
}