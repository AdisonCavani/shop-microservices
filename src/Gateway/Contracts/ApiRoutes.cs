namespace Gateway.Contracts;

public class ApiRoutes
{
    private const string BasePath = "/api";

    public const string Health = $"{BasePath}/health";

    public class User
    {
        public const string BasePath = $"{ApiRoutes.BasePath}/user";
        
        public const string Register = "/register";
        public const string Login = "/login";
        public const string VerifyEmail = "/verify-email";
        public const string Logout = "/logout";
    }
}