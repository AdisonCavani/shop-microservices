using Microsoft.Extensions.DependencyInjection;

namespace CoreShared.Startup;

public static class Swagger
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}