using FluentValidation;
using ProductService.Endpoints;
using ProductService.Startup;
// using Microsoft.EntityFrameworkCore;
// using ProductService.Database;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddSwagger();
builder.AddInfrastructure();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

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
app.MapGrpcHealthChecksService();
app.MapDefaultEndpoints();

app.Run();