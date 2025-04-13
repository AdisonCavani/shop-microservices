using Gateway.Contracts.Dtos;
using Gateway.Contracts.Requests;

namespace Gateway.Repositories;

public interface IUserRepository
{
    Task<UserDto?> FindUserByIdAsync(Guid id);
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<UserDto> RegisterAsync(RegisterReq req);
    Task<Tuple<JwtTokenDto, UserDto>> LoginAsync(LoginReq req);
    Task VerifyEmailAsync(Guid token);
    Task ResendVerifyEmailAsync(ResendVerifyEmailReq req);
}