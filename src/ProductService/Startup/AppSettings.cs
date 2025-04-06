using CoreShared.Settings;

namespace ProductService.Startup;

public class AppSettings : BaseAppSettings
{
    public required string PostgresConnectionString { get; init; }
}