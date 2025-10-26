var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithEndpoint(port: 5432, targetPort: 5432, name: "postgres");

var notificationsDb = postgres.AddDatabase("Notifications");
var productsDb = postgres.AddDatabase("Products");
var usersDb = postgres.AddDatabase("Users");
var ordersDb = postgres.AddDatabase("Orders");

var redis = builder.AddRedis("redis")
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

var identityService = builder.AddProject<Projects.Gateway>("identityService")
    .WithReference(rabbitmq)
    .WithReference(usersDb)
    .WithReference(redis)
    .WaitFor(rabbitmq)
    .WaitFor(usersDb)
    .WaitFor(redis);

var notificationService = builder.AddProject<Projects.NotificationService>("notificationService")
    .WithReference(rabbitmq)
    .WithReference(notificationsDb)
    .WithReference(identityService)
    .WaitFor(rabbitmq)
    .WaitFor(notificationsDb)
    .WaitFor(identityService);

var productService = builder.AddProject<Projects.ProductService>("productService")
    .WithReference(rabbitmq)
    .WithReference(productsDb)
    .WaitFor(rabbitmq)
    .WaitFor(productsDb);

var orderService = builder.AddProject<Projects.OrderService>("orderService")
    .WithReference(rabbitmq)
    .WithReference(productService)
    .WithReference(ordersDb)
    .WaitFor(rabbitmq)
    .WaitFor(productService)
    .WaitFor(ordersDb);

identityService
    .WithReference(notificationService)
    .WithReference(productService)
    .WithReference(orderService);

builder.Build().Run();