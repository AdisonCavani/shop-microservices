using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Server.Contracts.Dtos;
using Server.Contracts.Requests;

namespace Server.Repositories;

public interface IUserRepository
{
    Task<UserDto?> FindUserByIdAsync(Guid id);
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> RegisterAsync(RegisterReq req);
    Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> LoginAsync(LoginReq req);
    Task VerifyEmailAsync(VerifyEmailReq req);
    Task ResendVerifyEmailAsync(ResendVerifyEmailReq req);
}