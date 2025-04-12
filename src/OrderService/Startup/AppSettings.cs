using CoreShared.Settings;

namespace OrderService.Startup;

public class AppSettings : BaseAppSettings
{
    public required StripeSettings Stripe { get; init; }
}

public class StripeSettings
{
    public required string PublishableKey { get; init; }
    
    public required string SecretKey { get; init; }
    
    public required string WebhookSecret { get; init; }
}