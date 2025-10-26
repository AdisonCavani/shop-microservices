using CoreShared.Settings;
using CoreShared.Startup;
using FluentValidation;
using Gateway.Database;
using Microsoft.EntityFrameworkCore;
using Gateway.Endpoints;
using Gateway.Startup;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService;
using OrderService;
using ProductService;

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
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddServices();

var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";

builder.Services.AddGrpcServiceReference<NotificationAPI.NotificationAPIClient>($"{(isHttps ? "https" : "http")}://notificationService", failureStatus: HealthStatus.Degraded);
builder.Services.AddGrpcServiceReference<OrderAPI.OrderAPIClient>($"{(isHttps ? "https" : "http")}://orderService", failureStatus: HealthStatus.Degraded);
builder.Services.AddGrpcServiceReference<ProductAPI.ProductAPIClient>($"{(isHttps ? "https" : "http")}://productService", failureStatus: HealthStatus.Degraded);

builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetRequiredSection("Settings:ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway");
        options.SwaggerEndpoint("/swagger/product-service/v1/swagger.json", "ProductService");
        options.SwaggerEndpoint("/swagger/order-service/v1/swagger.json", "OrderService");
        options.SwaggerEndpoint("/swagger/notification-service/v1/swagger.json", "NotificationService");
    });
}

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

if (context.Database.IsRelational())
    await context.Database.MigrateAsync();

app.UseHsts();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();
app.MapDefaultEndpoints();
app.MapGrpcHealthChecksService();
app.MapReverseProxy();

app.Run();