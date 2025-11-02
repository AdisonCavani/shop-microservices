using OrderService.Services;

namespace OrderService.Startup;

public static class Services
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService.Services.OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
    }
}