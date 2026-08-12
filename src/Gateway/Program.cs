using CoreShared.Settings;
using CoreShared.Startup;
using FluentValidation;
using Gateway.Database;
using Microsoft.EntityFrameworkCore;
using Gateway.Endpoints;
using Gateway.Repositories;
using Gateway.Startup;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService;
using OrderService;
using ProductService;
using ProtobufSpec;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetailsHandling();

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetRequiredSection("Settings"));
var appSettings = builder.Configuration.GetRequiredSection("Settings").Get<AppSettings>()?.Validate()!;

// Add services to the container.
builder.Services.AddSwagger();
builder.AddInfrastructure();
builder.Services.AddAuth(appSettings);
builder.Services.AddServices();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";

builder.Services.AddHttpClient(ServiceDefinitions.Notification.Name, x =>
{
    x.BaseAddress = new Uri($"{(isHttps ? "https" : "http")}://{ServiceDefinitions.Notification.Name}");
});
builder.Services.AddHttpClient(ServiceDefinitions.Order.Name, x =>
{
    x.BaseAddress = new Uri($"{(isHttps ? "https" : "http")}://{ServiceDefinitions.Order.Name}");
});
builder.Services.AddHttpClient(ServiceDefinitions.Product.Name, x =>
{
    x.BaseAddress = new Uri($"{(isHttps ? "https" : "http")}://{ServiceDefinitions.Product.Name}");
});

builder.Services.AddGrpcServiceReference<NotificationAPI.NotificationAPIClient>(
    $"{(isHttps ? "https" : "http")}://{ServiceDefinitions.Notification.Name}",
    failureStatus: HealthStatus.Degraded);
builder.Services.AddGrpcServiceReference<OrderAPI.OrderAPIClient>(
    $"{(isHttps ? "https" : "http")}://{ServiceDefinitions.Order.Name}",
    failureStatus: HealthStatus.Degraded);
builder.Services.AddGrpcServiceReference<ProductAPI.ProductAPIClient>(
    $"{(isHttps ? "https" : "http")}://{ServiceDefinitions.Product.Name}",
    failureStatus: HealthStatus.Degraded);

builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetRequiredSection("Settings:ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseServiceDefaults();

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
app.MapDefaultEndpoints();
app.MapGrpcHealthChecksService();
app.MapReverseProxy();

app.Run();
