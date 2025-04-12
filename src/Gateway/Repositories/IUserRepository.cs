using System.Security.Claims;
using Gateway.Contracts.Dtos;
using Gateway.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;

namespace Gateway.Repositories;

public interface IUserRepository
{
    Task<UserDto?> FindUserByIdAsync(Guid id);
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> RegisterAsync(RegisterReq req);
    Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> LoginAsync(LoginReq req);
    Task VerifyEmailAsync(Guid token, Guid userId);
    Task ResendVerifyEmailAsync(ResendVerifyEmailReq req);
}