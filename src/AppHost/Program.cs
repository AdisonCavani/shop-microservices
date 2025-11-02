using ProtobufSpec;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ(ServiceDefinitions.RabbitMQ)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithEndpoint(port: 5432, targetPort: 5432, name: "postgres");

var identityDb = postgres.AddDatabase(ServiceDefinitions.Identity.Database);
var notificationsDb = postgres.AddDatabase(ServiceDefinitions.Notification.Database);
var productsDb = postgres.AddDatabase(ServiceDefinitions.Product.Database);
var ordersDb = postgres.AddDatabase(ServiceDefinitions.Order.Database);

var identityRedis = builder.AddRedis(ServiceDefinitions.Redis)
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

var identityService = builder.AddProject<Projects.Gateway>(ServiceDefinitions.Identity.Name)
    .WithReference(rabbitmq)
    .WithReference(identityDb)
    .WithReference(identityRedis)
    .WaitFor(rabbitmq)
    .WaitFor(identityDb)
    .WaitFor(identityRedis);

var notificationService = builder.AddProject<Projects.NotificationService>(ServiceDefinitions.Notification.Name)
    .WithReference(rabbitmq)
    .WithReference(notificationsDb)
    .WithReference(identityService)
    .WaitFor(rabbitmq)
    .WaitFor(notificationsDb)
    .WaitFor(identityService);

var productService = builder.AddProject<Projects.ProductService>(ServiceDefinitions.Product.Name)
    .WithReference(rabbitmq)
    .WithReference(productsDb)
    .WaitFor(rabbitmq)
    .WaitFor(productsDb);

var orderService = builder.AddProject<Projects.OrderService>(ServiceDefinitions.Order.Name)
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