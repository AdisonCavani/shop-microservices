using CoreShared.Settings;

namespace OrderService.Startup;

public class AppSettings : BaseAppSettings
{
    public required StripeSettings Stripe { get; init; }
}

public class StripeSettings
{
    public required string StripePublishableKey { get; init; }
    
    public required string StripeSecretKey { get; init; }
}