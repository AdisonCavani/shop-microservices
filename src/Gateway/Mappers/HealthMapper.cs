using Common;
using ProtobufSpec.Dtos;
using ProtobufSpec.Responses;

namespace Gateway.Mappers;

public static class HealthMapper
{
    public static HealthCheckDto ToHealthCheckDto(this HealthCheck proto)
    {
        return new HealthCheckDto
        {
            Status = proto.Status,
            Component = proto.Component,
            Description = proto.Description
        };
    }
    
    public static HealthCheckRes ToHealthCheckRes(this HealthResponse proto)
    {
        return new HealthCheckRes
        {
            ServiceName = proto.ServiceName,
            Status = proto.Status,
            Checks = proto.Checks.Select(check => check.ToHealthCheckDto()),
            Duration = TimeSpan.Parse(proto.Duration)
        };
    }
}