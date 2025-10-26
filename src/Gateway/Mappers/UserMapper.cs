using Gateway.Contracts.Requests;
using Gateway.Database.Entities;

namespace Gateway.Mappers;

public static class UserMapper
{
    public static UserDto ToUserDto(this UserEntity userEntity)
    {
        return new UserDto
        {
            Id = userEntity.Id.ToString(),
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
            Email = userEntity.Email,
            EmailConfirmed = userEntity.EmailConfirmed
        };
    }

    public static UserEntity ToUserEntity(this RegisterReq registerReq)
    {
        return new UserEntity
        {
            FirstName = registerReq.FirstName,
            LastName = registerReq.LastName,
            Email = registerReq.Email.ToLower(),
            Password = registerReq.Password
        };
    }
}