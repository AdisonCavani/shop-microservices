using CoreShared.Settings;
using OrderService.Startup;
using Stripe;
using Stripe.Checkout;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Settings"));

var appSettings = builder.Configuration.GetRequiredSection("Settings").Get<AppSettings>()?.Validate()!;

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

StripeConfiguration.ApiKey = appSettings.Stripe.StripeSecretKey;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/order", async () =>
    {
        var options = new SessionCreateOptions
        {
            SuccessUrl = "",
            CancelUrl = "",
            LineItems = new(),
            Mode = "payment",
            CustomerEmail = "xd@gmail.com"
        };

        foreach (var item in Array.Empty<object>())
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = 1 * 1,
                    Currency = "PLN",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "",
                        Description = "",
                    }
                },
                Quantity = 1
            });
        }
        
        var checkoutSession = await new SessionService().CreateAsync(options);

        return TypedResults.Redirect(checkoutSession.Url, false);
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();