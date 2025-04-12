using CoreShared.Startup;
using FluentValidation;
using ProductService.Database;
using ProductService.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

// Add services to the container.
builder.Services.AddSwagger();
builder.AddNpgsqlDbContext<AppDbContext>("Products");
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

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

if (context.Database.IsRelational())
    await context.Database.MigrateAsync();

app.UseHsts();
app.UseHttpsRedirection();

app.MapEndpoints();
app.MapGrpcHealthChecksService();
app.MapDefaultEndpoints();

app.Run();