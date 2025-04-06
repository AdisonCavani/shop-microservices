using CoreShared.Settings;
using ProtoBuf;
using Yarp.ReverseProxy.Configuration;

namespace Gateway.Startup;

public class AppSettings : BaseAppSettings
{
    public required string PostgresConnectionString { get; init; }
    
    public required string RedisConnectionString { get; init; }
    
    public required ReverseProxySettings ReverseProxy { get; init; }
}

public class ReverseProxySettings
{
    public required Dictionary<string, RouteConfig> Routes { get; init; }
    
    public required Dictionary<string, ClusterConfig> Clusters { get; init; }
    
    public required Dictionary<string, GatewaySwaggerSpec> Swagger { get; init; }
}

public class GatewaySwaggerSpec
{
    public required string Endpoint { get; init; }
    
    public required string Spec { get; init; }
    
    public required string OriginPath { get; init; }
    
    public required string TargetPath { get; init; }
}
