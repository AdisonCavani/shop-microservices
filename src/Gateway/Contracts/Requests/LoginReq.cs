namespace Gateway.Contracts.Requests;

public class LoginReq
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool Persistent { get; set; } = false;
}