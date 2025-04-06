using CoreShared.Settings;
using Gateway.Database;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Gateway.Endpoints;
using Gateway.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetRequiredSection("Settings"));

var appSettings = builder.Configuration.GetRequiredSection("Settings").Get<AppSettings>()?.Validate()!;

// Add services to the container.
builder.Services.AddSwagger();
builder.Services.AddInfrastructure(appSettings);
builder.Services.AddAuth();
builder.Services.AddValidators();
builder.Services.AddServices();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetRequiredSection("Settings:ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.MapReverseProxy();

app.Run();