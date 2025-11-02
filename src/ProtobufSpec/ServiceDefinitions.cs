namespace ProtobufSpec;

public class ServiceDefinitions
{
    public const string RabbitMQ = "rabbitmq";
    public const string Redis = "redis";
    
    public class Identity
    {
        public const string Name = "identity-service";

        public const string Database = $"{Name}-db";
    }
    
    public class Notification
    {
        public const string Name = "notification-service";

        public const string Database = $"{Name}-db";
    }
    
    public class Order
    {
        public const string Name = "order-service";

        public const string Database = $"{Name}-db";
    }
    
    public class Product
    {
        public const string Name = "product-service";

        public const string Database = $"{Name}-db";
    }
}