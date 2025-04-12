using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Yarp.ReverseProxy.Configuration;

namespace Gateway.Startup;

public static class Swagger
{
    // https://github.com/dotnet/yarp/issues/1789#issuecomment-1263409031
    public static void MapGetSwaggerForYarp(this IEndpointRouteBuilder endpoints)
    {
        using var scope = endpoints.ServiceProvider.CreateScope();
        var appSettings = scope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>();
        
        var clusters = appSettings.Value.ReverseProxy.Clusters;
        var routes = appSettings.Value.ReverseProxy.Routes;
        
        foreach (var (clusterId, _) in clusters)
        {
            appSettings.Value.ReverseProxy.Swagger.TryGetValue(clusterId, out var swagger);

            // Map swagger endpoint if we find a cluster with swagger configuration
            if (swagger is not null)
                endpoints.MapSwaggerSpecs(routes, clusterId, swagger);
        }
    }

    private static void MapSwaggerSpecs(
        this IEndpointRouteBuilder endpoints,
        Dictionary<string, RouteConfig> config,
        string clusterId, 
        GatewaySwaggerSpec swagger)
    {
        endpoints.Map(swagger.Endpoint, async context =>
        {
            var client = new HttpClient();
            var stream = await client.GetStreamAsync(swagger.Spec);

            var document = new OpenApiStreamReader().Read(stream, out _);
            var rewrite = new OpenApiPaths();

            // map existing path
            config.TryGetValue(clusterId, out var routes);

            if (routes is null)
                return;
            
            var hasCatchAll = routes.Match.Path?.Contains("**catch-all") ?? false;

            foreach (var path in document.Paths)
            {

                var rewritedPath = path.Key.Replace(swagger.TargetPath, swagger.OriginPath);

                // there is a catch all, all route are elligible 
                // or there is a route that match without any operation methods filtering
                if (hasCatchAll || routes.Match.Path!.Equals(rewritedPath) && routes.Match.Methods == null)
                    rewrite.Add(rewritedPath, path.Value);
                else
                {
                    // there is a route that match
                    var routeThatMatchPath = routes.Match.Path?.Equals(rewritedPath) ?? false;
                    if (routeThatMatchPath)
                    {
                        // filter operation based on yarp config
                        var operationToRemoves = new List<OperationType>();
                        foreach (var operation in path.Value.Operations)
                        {
                            // match route and method
                            var hasRoute = routes.Match.Path!.Equals(rewritedPath) &&
                                           routes.Match.Methods!.Contains(operation.Key.ToString().ToUpperInvariant());

                            if (!hasRoute)
                            {
                                operationToRemoves.Add(operation.Key);
                            }
                        }

                        // remove operation routes
                        foreach (var operationToRemove in operationToRemoves)
                        {
                            path.Value.Operations.Remove(operationToRemove);
                        }

                        // add path if there is any operation
                        if (path.Value.Operations.Any())
                        {
                            rewrite.Add(rewritedPath, path.Value);
                        }
                    }
                }
            }

            document.Paths = rewrite;

            var result = document.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);
            await context.Response.WriteAsync(result);
        });
    }
}