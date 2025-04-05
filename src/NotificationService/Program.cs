using Fluid;
using Microsoft.Extensions.FileProviders;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Templates")));
builder.Services.AddSingleton<FluidParser>();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EmailHandler>();

builder.Services.AddHostedService<MessageBusSubscriber>();

var app = builder.Build();

app.Run();