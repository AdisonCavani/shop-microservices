using System.Security.Claims;
using Gateway.Auth;
using Gateway.Contracts.Dtos;
using Gateway.Contracts.Requests;
using Gateway.Database;
using Gateway.Database.Entities;
using Gateway.Endpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProtobufSpec.Events;
using StackExchange.Redis;
using CoreShared.Transit;
using Gateway.Mappers;

namespace Gateway.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly Publisher<ConfirmEmailEvent> _publisher;
    private readonly IPasswordHasher<UserEntity> _hasher;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public UserRepository(
        AppDbContext context,
        IPasswordHasher<UserEntity> hasher,
        Publisher<ConfirmEmailEvent> publisher,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _context = context;
        _hasher = hasher;
        _publisher = publisher;
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<UserDto?> FindUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user?.ToUserDto();
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstAsync(x => x.Id == id);
        return user.ToUserDto();
    }

    public async Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> RegisterAsync(RegisterReq req)
    {
        var user = req.ToUserEntity();

        var hashedPassword = _hasher.HashPassword(user, req.Password);
        user.Password = hashedPassword;

        _context.Users.Add(user);
        var result = await _context.SaveChangesAsync();

        if (result <= 0)
            throw new Exception(ExceptionMessages.DatabaseProblem);

        await SendEmailVerifyAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow.AddYears(2),
            IsPersistent = true
        };

        return Tuple.Create(claims, authProperties, user.ToUserDto());
    }

    public async Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> LoginAsync(LoginReq req)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email.ToLower());

        if (user is null)
            throw new Exception(ExceptionMessages.EmailNotExist);

        var valid = _hasher.VerifyHashedPassword(user, user.Password, req.Password);

        if (valid == PasswordVerificationResult.Failed)
            throw new Exception(ExceptionMessages.PasswordInvalid);

        // TODO
        // if (valid == PasswordVerificationResult.SuccessRehashNeeded)

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow.AddYears(2),
            IsPersistent = req.Persistent
        };

        return Tuple.Create(claims, authProperties, user.ToUserDto());
    }

    public async Task VerifyEmailAsync(Guid token, Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
            throw new Exception();

        if (user.EmailConfirmed)
            throw new Exception();

        var db = _connectionMultiplexer.GetDatabase();
        var bytes = db.StringGet(AuthSchema.VerifyEmailKeyPrefix + token);

        if (!bytes.HasValue)
            throw new Exception();

        if (userId != Guid.Parse(bytes!))
            throw new Exception();

        user.EmailConfirmed = true;
        var result = await _context.SaveChangesAsync();
        
        if (result <= 0)
            throw new Exception(ExceptionMessages.DatabaseProblem);
    }

    public async Task ResendVerifyEmailAsync(ResendVerifyEmailReq req)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == req.Id);

        if (user is null)
            throw new Exception();
        
        await SendEmailVerifyAsync(user);
    }
    
    private async Task SendEmailVerifyAsync(UserEntity user)
    {
        var eventMessage = user.ToConfirmEmailEvent();

        var db = _connectionMultiplexer.GetDatabase();
        db.StringSet(AuthSchema.VerifyEmailKeyPrefix + eventMessage.Token, user.Id.ToString(), TimeSpan.FromHours(12));

        await _publisher.PublishEventAsync(eventMessage);
    }
}