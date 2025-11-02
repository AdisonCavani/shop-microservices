using CoreShared.Settings;
using Yarp.ReverseProxy.Configuration;

namespace Gateway.Startup;

public class AppSettings : BaseAppSettings
{
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
    public required string OriginPath { get; init; }
    
    public required string TargetPath { get; init; }
}
