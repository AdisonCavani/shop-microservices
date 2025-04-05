using FluentValidation;
using Server.Auth;
using Server.Database;
using Server.Database.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Server.Contracts.Requests;
using Server.Endpoints;
using Server.Repositories;
using Server.Services;
using Server.Settings;
using Server.Validators;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbSettings = builder.Configuration.GetSection(nameof(DbSettings)).Get<DbSettings>()!.Validate();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IValidator<LoginReq>, LoginReqValidator>();
builder.Services.AddScoped<IValidator<RegisterReq>, RegisterReqValidator>();
builder.Services.AddScoped<IValidator<VerifyEmailReq>, VerifyEmailReqValidator>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(dbSettings.RedisConnectionString));
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseNpgsql(dbSettings.PostgresConnectionString,
        npgSettings => npgSettings.EnableRetryOnFailure());
});
builder.Services
    .AddSingleton<IConnection>(_ => new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5672
    }.CreateConnection())
    .AddHealthChecks()
    .AddNpgSql(dbSettings.PostgresConnectionString)
    .AddRedis(dbSettings.RedisConnectionString)
    .AddRabbitMQ();

builder.Services.AddSingleton<MessageBusPublisher>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddSingleton<ITicketStore, RedisTicketStore>();
builder.Services
    .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure<ITicketStore>((options, store) =>
    {
        options.SessionStore = store;
        options.Cookie.Name = AuthSchema.CookieName;
    });
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

app.Run();