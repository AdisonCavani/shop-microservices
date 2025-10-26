namespace ProtobufSpec;

public class ApiRoutes
{
    private const string BasePath = "/api";

    public const string Health = $"{BasePath}/health";

    public class User
    {
        public const string Path = $"{BasePath}/user";
        
        public const string Register = "/register";
        public const string Login = "/login";
        public const string VerifyEmail = "/verify-email/{token}";
    }

    public class Product
    {
        public const string Path = $"{BasePath}/product";
        
        public const string Get = "/{productId}";
    }

    public class Order
    {
        public const string Path = $"{BasePath}/order";
        
        public const string Get = "/{orderId}";
    }
    
    public class Payment
    {
        public const string Path = $"{BasePath}/payment";
        
        public const string Get = "/{orderId}";
        public const string Webhook = "/webhook";
    }

    public class Notification
    {
        public const string Path = $"{BasePath}/notification";
        
        public const string ById = "/{triggerName}";
    }
}