using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoreShared;

public class AuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var hasAuthorize = endpointMetadata?.OfType<IAuthorizeData>().Any() == true;

        if (!hasAuthorize)
            return;

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme),
                new List<string>()
            }
        };
        operation.Security = new List<OpenApiSecurityRequirement> { securityRequirement };
        operation.Responses.TryAdd(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse { Description = "Forbidden" });
    }
}
