namespace Server.Contracts;

public class ApiRoutes
{
    private const string BasePath = "/api";

    public const string Health = $"{BasePath}/health";

    public class User
    {
        public const string BasePath = $"{ApiRoutes.BasePath}/user";
        
        public const string Register = $"{BasePath}/register";
        public const string Login = $"{BasePath}/login";
        public const string VerifyEmail = $"{BasePath}/verify-email";
        public const string Logout = $"{BasePath}/logout";
    }
}