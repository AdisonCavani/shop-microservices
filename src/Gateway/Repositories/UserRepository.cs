using CoreShared;
using Gateway.Auth;
using Gateway.Contracts.Dtos;
using Gateway.Contracts.Requests;
using Gateway.Database;
using Gateway.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProtobufSpec.Events;
using StackExchange.Redis;
using CoreShared.Transit;
using Gateway.Mappers;

namespace Gateway.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly Publisher<ConfirmEmailEvent> _publisher;
    private readonly IPasswordHasher<UserEntity> _hasher;
    private readonly JwtService _jwtService;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public UserRepository(
        AppDbContext dbContext,
        IPasswordHasher<UserEntity> hasher,
        Publisher<ConfirmEmailEvent> publisher,
        IConnectionMultiplexer connectionMultiplexer,
        JwtService jwtService)
    {
        _dbContext = dbContext;
        _hasher = hasher;
        _publisher = publisher;
        _jwtService = jwtService;
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<UserDto?> FindUserByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user?.ToUserDto();
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstAsync(x => x.Id == id);
        return user.ToUserDto();
    }

    public async Task<UserDto> RegisterAsync(RegisterReq req)
    {
        var user = req.ToUserEntity();

        var hashedPassword = _hasher.HashPassword(user, req.Password);
        user.Password = hashedPassword;

        _dbContext.Users.Add(user);
        var result = await _dbContext.SaveChangesAsync();

        if (result <= 0)
            throw new Exception(ExceptionMessages.DatabaseProblem);

        await SendEmailVerifyAsync(user);

        return user.ToUserDto();
    }

    public async Task<Tuple<JwtTokenDto, UserDto>> LoginAsync(LoginReq req)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == req.Email.ToLower());

        if (user is null)
            throw new Exception(ExceptionMessages.EmailNotExist);

        var valid = _hasher.VerifyHashedPassword(user, user.Password, req.Password);

        if (valid == PasswordVerificationResult.Failed)
            throw new Exception(ExceptionMessages.PasswordInvalid);

        // TODO
        // if (valid == PasswordVerificationResult.SuccessRehashNeeded)

        if (!user.EmailConfirmed)
            throw new Exception(ExceptionMessages.EmailNotConfirmed);
        
        var token = _jwtService.GenerateToken(user);

        return Tuple.Create(token, user.ToUserDto());
    }

    public async Task VerifyEmailAsync(Guid token)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var bytes = db.StringGet(AuthSchema.VerifyEmailKeyPrefix + token);

        if (!bytes.HasValue)
            throw new Exception();

        var userId = Guid.Parse(bytes!);
        
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        
        if (user is null)
            throw new Exception(ExceptionMessages.MissingUserForVerificationToken);

        user.EmailConfirmed = true;
        var result = await _dbContext.SaveChangesAsync();
        
        if (result <= 0)
            throw new Exception(ExceptionMessages.DatabaseProblem);
    }

    public async Task ResendVerifyEmailAsync(ResendVerifyEmailReq req)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == req.Id);

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