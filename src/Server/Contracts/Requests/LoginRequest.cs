namespace Server.Contracts.Requests;

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password{ get; set; }
    public bool Persistent { get; set; } = false;
}