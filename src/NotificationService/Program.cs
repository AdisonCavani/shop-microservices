using CoreShared.Startup;
using Gateway;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService.Services;
using NotificationService.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetailsHandling();

builder.AddInfrastructure();
builder.Services.AddServices(builder.Environment);
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();

var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";

builder.Services.AddGrpcServiceReference<IdentityAPI.IdentityAPIClient>($"{(isHttps ? "https" : "http")}://identityService", failureStatus: HealthStatus.Degraded);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGrpcService<NotificationGrpcService>();
app.MapGrpcHealthChecksService();
app.MapDefaultEndpoints();

app.Run();