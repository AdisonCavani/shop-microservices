using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Server.Contracts.Dtos;
using Server.Contracts.Requests;

namespace Server.Repositories;

public interface IUserRepository
{
    Task<UserDto?> FindUserByIdAsync(Guid id);
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> RegisterAsync(RegisterRequest req);
    Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> LoginAsync(LoginRequest req);
    Task VerifyEmailAsync(VerifyEmailRequest req);
    Task ResendVerifyEmailAsync(ResendVerifyEmailRequest req);
}