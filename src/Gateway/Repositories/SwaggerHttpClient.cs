using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Gateway.Repositories;

public class SwaggerHttpClient(HttpClient client)
{
    public async Task<OpenApiDocument> GetSwaggerDocumentAsync()
    {
        var stream = await client.GetStreamAsync("/swagger/v1/swagger.json");
        return new OpenApiStreamReader().Read(stream, out _);
    }
}
