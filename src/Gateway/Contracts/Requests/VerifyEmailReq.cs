namespace Gateway.Contracts.Requests;

public class VerifyEmailReq
{
    public Guid Id { get; set; }
    
    public Guid Token { get; set; }
}