// using Microsoft.EntityFrameworkCore;
// using ProductService.Database;
using ProductService.Endpoints;
using ProductService.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetRequiredSection("Settings"));

var appSettings = builder.Configuration.GetRequiredSection("Settings").Get<AppSettings>()?.Validate()!;

// Add services to the container.
builder.Services.AddSwagger();
builder.Services.AddInfrastructure(appSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// using var scope = app.Services.CreateScope();
// var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//
// if (context.Database.IsRelational())
//     context.Database.Migrate();

app.UseHsts();
app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();