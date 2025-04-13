using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoreShared;

public class AuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var hasAuthorize = endpointMetadata.OfType<IAuthorizeData>().Any();

        if (!hasAuthorize)
            return;

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new()
                {
                    Reference = new()
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }
        };
        operation.Security = new List<OpenApiSecurityRequirement> { securityRequirement };
        operation.Responses.TryAdd(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse { Description = "Forbidden" });
    }
}