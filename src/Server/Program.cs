using AppAny.HotChocolate.FluentValidation;
using FluentValidation.AspNetCore;
using Server.Auth;
using Server.Database;
using Server.Database.Entities;
using Server.Resolvers.Mutations;
using Server.Resolvers.Queries;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Settings;
using Server.Validators;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbSettings = builder.Configuration.GetSection(nameof(DbSettings)).Get<DbSettings>()!.Validate();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddTransient<LoginRequestValidator>();
builder.Services.AddTransient<RegisterRequestValidator>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(dbSettings.RedisConnectionString));
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseNpgsql(dbSettings.PostgresConnectionString,
        npgSettings => npgSettings.EnableRetryOnFailure());
});

builder.Services.AddHttpContextAccessor();
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<UserQuery>()
    .AddTypeExtension<UserMutation>()
    .AddAuthorization()
    .AddFluentValidation();

builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddSingleton<ITicketStore, RedisTicketStore>();
builder.Services
    .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure<ITicketStore>((options, store) =>
    {
        options.SessionStore = store;
        options.Cookie.Name = CookieSchema.CookieName;
    });
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

var app = builder.Build();

// Configure the HTTP request pipeline.
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

if (context.Database.IsRelational())
    context.Database.Migrate();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCookiePolicy(new()
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    HttpOnly = HttpOnlyPolicy.Always
});

app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL().WithOptions(new()
    {
        Tool =
        {
            Enable = app.Environment.IsDevelopment(),
            DisableTelemetry = true,
            IncludeCookies = true
        }
    });
});

app.Run();