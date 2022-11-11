namespace Server.Templates;

public class VerifyEmail
{
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string Token { get; set; }

    public const string Subject = "Verify your email";
}