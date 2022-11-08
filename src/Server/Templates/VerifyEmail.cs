namespace Server.Templates;

public class VerifyEmail
{
    public required string Name { get; set; }
    
    public required string Token { get; set; }

    public const string Subject = "Verify your email";
}