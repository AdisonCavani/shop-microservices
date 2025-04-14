var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithEndpoint(port: 5432, targetPort: 5432, name: "postgres");

var productsDb = postgres.AddDatabase("Products");
var usersDb = postgres.AddDatabase("Users");
var ordersDb = postgres.AddDatabase("Orders");

var redis = builder.AddRedis("redis")
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

var notificationService = builder.AddProject<Projects.NotificationService>("notificationService")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

var productService = builder.AddProject<Projects.ProductService>("productService")
    .WithReference(productsDb)
    .WaitFor(productsDb);

var orderService = builder.AddProject<Projects.OrderService>("orderService")
    .WithReference(productService)
    .WithReference(ordersDb)
    .WithReference(rabbitmq)
    .WaitFor(productService)
    .WaitFor(ordersDb)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.Gateway>("gateway")
    .WithReference(rabbitmq)
    .WithReference(usersDb)
    .WithReference(redis)
    .WithReference(notificationService)
    .WithReference(orderService)
    .WithReference(productService)
    .WaitFor(rabbitmq)
    .WaitFor(usersDb)
    .WaitFor(redis)
    .WaitFor(notificationService)
    .WaitFor(orderService)
    .WaitFor(productService);

builder.Build().Run();