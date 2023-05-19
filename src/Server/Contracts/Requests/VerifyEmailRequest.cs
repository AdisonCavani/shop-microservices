namespace Server.Contracts.Requests;

public class VerifyEmailRequest
{
    public Guid Id { get; set; }
    
    public Guid Token { get; set; }
}