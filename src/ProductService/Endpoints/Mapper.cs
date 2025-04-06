namespace ProductService.Endpoints;

public static class Mapper
{
    private static void MapUserApi(this RouteGroupBuilder group)
    {
        

        group.WithTags("User endpoint");
    }
    
    public static void MapEndpoints(this WebApplication app)
    {
        
    }
}