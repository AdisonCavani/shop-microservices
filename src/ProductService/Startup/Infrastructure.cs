using ProductService.Database;

namespace ProductService.Startup;

public static class Infrastucture
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>("Products");
    }
}