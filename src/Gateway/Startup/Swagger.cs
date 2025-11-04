using Gateway.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
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
        endpoints.Map($"/swagger/{clusterId}/v1/swagger.json", async context =>
        {
            var clientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
            var client = new SwaggerHttpClient(clientFactory.CreateClient(clusterId));

            var document = await client.GetSwaggerDocumentAsync();
            var rewrite = new OpenApiPaths();

            var routeConfigs = config
                .Where(kvp => kvp.Key == clusterId || kvp.Key.StartsWith($"{clusterId}_"))
                .Select(kvp => kvp.Value)
                .ToList();

            if (!routeConfigs.Any())
                return;

            foreach (var path in document.Paths)
            {
                // Replace the target path with the origin path.
                var rewritedPath = path.Key.Replace(swagger.TargetPath, swagger.OriginPath);

                // Try to get a matching route configuration for this rewritten path.
                // This example takes the first route that matches.
                var matchingRoute = routeConfigs.FirstOrDefault(rc =>
                    rc.Match.Path != null &&
                    rc.Match.Path.Equals(rewritedPath, StringComparison.OrdinalIgnoreCase));

                // If no route config matches exactly, check if any route is using a catch-all pattern.
                if (matchingRoute == null)
                {
                    matchingRoute = routeConfigs.FirstOrDefault(rc =>
                        rc.Match.Path?.Contains("**catch-all", StringComparison.OrdinalIgnoreCase) == true);
                }

                if (matchingRoute is null)
                {
                    // If still no matching route config, skip this path.
                    continue;
                }

                // Determine if the route configuration uses a catch-all.
                var hasCatchAll =
                    matchingRoute.Match.Path?.Contains("**catch-all", StringComparison.OrdinalIgnoreCase) ?? false;

                if (hasCatchAll ||
                    (matchingRoute.Match.Path!.Equals(rewritedPath, StringComparison.OrdinalIgnoreCase) &&
                     matchingRoute.Match.Methods == null))
                {
                    rewrite.Add(rewritedPath, path.Value);
                }
                else
                {
                    // Check for an exact match on the rewritten path
                    var routeThatMatchPath =
                        matchingRoute.Match.Path?.Equals(rewritedPath, StringComparison.OrdinalIgnoreCase) ?? false;
                    if (routeThatMatchPath)
                    {
                        // Filter operations based on the allowed HTTP methods from YARP config.
                        var operationsToRemove = new List<OperationType>();
                        foreach (var operation in path.Value.Operations)
                        {
                            var hasRoute =
                                matchingRoute.Match.Path!.Equals(rewritedPath, StringComparison.OrdinalIgnoreCase) &&
                                matchingRoute.Match.Methods != null &&
                                matchingRoute.Match.Methods.Contains(operation.Key.ToString().ToUpperInvariant());
                            if (!hasRoute)
                            {
                                operationsToRemove.Add(operation.Key);
                            }
                        }

                        // Remove non-matching operations.
                        foreach (var operationToRemove in operationsToRemove)
                        {
                            path.Value.Operations.Remove(operationToRemove);
                        }

                        // Only add the path if there remains at least one valid operation.
                        if (path.Value.Operations.Any())
                        {
                            rewrite.Add(rewritedPath, path.Value);
                        }
                    }
                }
                
                bool requiresAuth = !string.Equals(
                    matchingRoute.AuthorizationPolicy, "anonymous", StringComparison.OrdinalIgnoreCase);

                if (requiresAuth)
                {
                    foreach (var operation in path.Value.Operations.Values)
                    {
                        operation.Responses.TryAdd(
                            StatusCodes.Status401Unauthorized.ToString(),
                            new OpenApiResponse { Description = "Unauthorized" }
                        );
                        operation.Responses.TryAdd(
                            StatusCodes.Status403Forbidden.ToString(),
                            new OpenApiResponse { Description = "Forbidden" }
                        );

                        operation.Security = new List<OpenApiSecurityRequirement>
                        {
                            new()
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "Bearer"
                                        }
                                    },
                                    Array.Empty<string>()
                                }
                            }
                        };
                    }
                }
            }

            document.Paths = rewrite;

            var result = document.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);
            await context.Response.WriteAsync(result);
        });
    }
}