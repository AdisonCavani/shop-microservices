var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var productsDb = postgres.AddDatabase("Products");
var usersDb = postgres.AddDatabase("Users");

var redis = builder.AddRedis("redis")
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

var notificationService = builder.AddProject<Projects.NotificationService>("notificationService")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

var orderService = builder.AddProject<Projects.OrderService>("orderService");

var productService = builder.AddProject<Projects.ProductService>("productService")
    .WithReference(productsDb)
    .WaitFor(productsDb);

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