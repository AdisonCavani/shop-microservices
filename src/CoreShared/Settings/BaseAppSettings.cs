namespace CoreShared.Settings;

public abstract class BaseAppSettings
{
    public required AuthSettings Auth { get; init; }
}

public class AuthSettings
{
    public required string Issuer { get; init; }
    
    public required string Audience { get; init; }

    public required string Secret { get; init; }
    
    public required int ExpireMinutes { get; init; }
}
