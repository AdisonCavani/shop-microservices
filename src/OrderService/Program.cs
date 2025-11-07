using CoreShared.Settings;
using CoreShared.Startup;
using FluentValidation;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OrderService.Database;
using OrderService.Endpoints;
using OrderService.Startup;
using ProductService;
using Stripe;
using Microsoft.EntityFrameworkCore;
using ProtobufSpec;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetailsHandling();

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Settings"));
var appSettings = builder.Configuration.GetRequiredSection("Settings").Get<AppSettings>()?.Validate()!;

// Add services to the container.
builder.Services.AddSwagger();
builder.AddInfrastructure();
builder.Services.AddAuth(appSettings);
builder.Services.AddServices();
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

StripeConfiguration.ApiKey = appSettings.Stripe.SecretKey;

var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";
builder.Services.AddGrpcServiceReference<ProductAPI.ProductAPIClient>($"{(isHttps ? "https" : "http")}://{ServiceDefinitions.Product.Name}", failureStatus: HealthStatus.Degraded);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

if (context.Database.IsRelational())
    await context.Database.MigrateAsync();

app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();
app.MapGrpcHealthChecksService();
app.MapDefaultEndpoints();

app.Run();