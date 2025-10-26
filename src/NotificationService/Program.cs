using CoreShared.Settings;
using CoreShared.Startup;
using FluentValidation;
using Gateway;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService.Database;
using NotificationService.Endpoints;
using NotificationService.Startup;

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
builder.Services.AddServices(builder.Environment);
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();

var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";

builder.Services.AddGrpcServiceReference<IdentityAPI.IdentityAPIClient>($"{(isHttps ? "https" : "http")}://identityService", failureStatus: HealthStatus.Degraded);

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

app.UseHsts();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();
app.MapGrpcHealthChecksService();
app.MapDefaultEndpoints();

app.Run();