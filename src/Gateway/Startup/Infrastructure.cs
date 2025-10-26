﻿using Gateway.Database;
using Microsoft.EntityFrameworkCore;
using ProtobufSpec;

namespace Gateway.Startup;

public static class Infrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>("Users", configureDbContextOptions: (options) =>
            options.UseNpgsql(npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<UserRoleEnum>("UserRoleEnum");
            }));
        builder.AddRedisClient("redis");
        builder.AddMassTransitRabbitMq("rabbitmq");
    }
}