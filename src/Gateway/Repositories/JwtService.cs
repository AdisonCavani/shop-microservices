using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gateway.Contracts.Dtos;
using Gateway.Database.Entities;
using Gateway.Startup;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Repositories;

public class JwtService
{
    private readonly IOptions<AppSettings> _appSettings;

    public JwtService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public JwtTokenDto GenerateToken(UserEntity user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.Auth.Secret));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new([
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new(ClaimTypes.Role, user.UserRole.ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_appSettings.Value.Auth.ExpireMinutes),
            Audience = _appSettings.Value.Auth.Audience,
            Issuer = _appSettings.Value.Auth.Issuer,
            SigningCredentials = new(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new()
        {
            Token = tokenHandler.WriteToken(token)
        };
    }
}