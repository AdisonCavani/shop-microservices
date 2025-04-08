using NotificationService.Services;
using NotificationService.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddServices(builder.Environment);
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<NotificationGrpcService>();

app.Run();