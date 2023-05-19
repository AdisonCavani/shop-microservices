using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Auth;
using Server.Contracts.Dtos;
using Server.Contracts.Events;
using Server.Contracts.Requests;
using Server.Database;
using Server.Database.Entities;
using Server.Resolvers;
using Server.Services;
using StackExchange.Redis;

namespace Server.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly MessageBusPublisher _publisher;
    private readonly IPasswordHasher<UserEntity> _hasher;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public UserRepository(
        IMapper mapper,
        AppDbContext context,
        IPasswordHasher<UserEntity> hasher,
        MessageBusPublisher publisher,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _mapper = mapper;
        _context = context;
        _hasher = hasher;
        _publisher = publisher;
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<UserDto?> FindUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user is null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstAsync(x => x.Id == id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> RegisterAsync(RegisterRequest req)
    {
        var user = _mapper.Map<UserEntity>(req);

        var hashedPassword = _hasher.HashPassword(user, req.Password);
        user.Password = hashedPassword;

        _context.Users.Add(user);
        var result = await _context.SaveChangesAsync();

        if (result <= 0)
            throw new Exception(ExceptionMessages.DatabaseProblem);

        SendEmailVerify(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow.AddYears(2),
            IsPersistent = true
        };

        return Tuple.Create(claims, authProperties, _mapper.Map<UserDto>(user));
    }

    public async Task<Tuple<List<Claim>, AuthenticationProperties, UserDto>> LoginAsync(LoginRequest req)
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

        return Tuple.Create(claims, authProperties, _mapper.Map<UserDto>(user));
    }

    public async Task VerifyEmailAsync(VerifyEmailRequest req)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == req.Id);

        if (user is null)
            throw new Exception();

        if (user.EmailConfirmed)
            throw new Exception();

        var db = _connectionMultiplexer.GetDatabase();
        var bytes = db.StringGet(AuthSchema.VerifyEmailKeyPrefix + req.Token);

        if (!bytes.HasValue)
            throw new Exception();

        if (req.Id != Guid.Parse(bytes!))
            throw new Exception();

        user.EmailConfirmed = true;
        var result = await _context.SaveChangesAsync();
        
        if (result <= 0)
            throw new GraphQLException(ExceptionMessages.DatabaseProblem);
    }

    public async Task ResendVerifyEmailAsync(ResendVerifyEmailRequest req)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == req.Id);

        if (user is null)
            throw new Exception();
        
        SendEmailVerify(user);
    }
    
    private void SendEmailVerify(UserEntity user)
    {
        var eventMessage = _mapper.Map<ConfirmEmailEvent>(user);

        var db = _connectionMultiplexer.GetDatabase();
        db.StringSet(AuthSchema.VerifyEmailKeyPrefix + eventMessage.Token, user.Id.ToString(), TimeSpan.FromHours(12));

        _publisher.PublishEvent(eventMessage);
    }
}