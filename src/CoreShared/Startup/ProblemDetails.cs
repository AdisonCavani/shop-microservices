using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace CoreShared.Startup;

public static class ProblemDetails
{
    public static void AddProblemDetailsHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
        
                var httpActivity = context.HttpContext.Features.Get<IHttpActivityFeature>();
                context.ProblemDetails.Extensions.TryAdd("traceId", httpActivity?.Activity.Id);
            };
        });
        services.AddExceptionHandler<ProblemExceptionHandler>();
    }
}