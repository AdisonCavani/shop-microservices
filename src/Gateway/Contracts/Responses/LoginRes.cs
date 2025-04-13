using Gateway.Contracts.Dtos;

namespace Gateway.Contracts.Responses;

public class LoginRes
{
    public required UserDto User { get; set; }
    
    public required JwtTokenDto JwtToken { get; set; }
}