using NotificationService.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddServices(builder.Environment);

var app = builder.Build();

app.Run();