using FluentValidation;
using Gateway.Database;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Gateway.Endpoints;
using Gateway.Startup;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService;
using ProductService;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetRequiredSection("Settings"));

// Add services to the container.
builder.Services.AddSwagger();
builder.AddInfrastructure();
builder.Services.AddAuth();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddServices();

var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";

builder.Services.AddGrpcServiceReference<NotificationAPI.NotificationAPIClient>($"{(isHttps ? "https" : "http")}://notificationService", failureStatus: HealthStatus.Degraded);
builder.Services.AddGrpcServiceReference<ProductAPI.ProductAPIClient>($"{(isHttps ? "https" : "http")}://productService", failureStatus: HealthStatus.Degraded);

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
    });
}

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

if (context.Database.IsRelational())
    context.Database.Migrate();

app.UseHsts();
app.UseHttpsRedirection();

app.UseCookiePolicy(new()
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    HttpOnly = HttpOnlyPolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();
app.MapDefaultEndpoints();
app.MapReverseProxy();

app.Run();