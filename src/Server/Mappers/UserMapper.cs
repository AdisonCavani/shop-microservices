using AutoMapper;
using Server.Contracts.Dtos;
using Server.Contracts.Events;
using Server.Database.Entities;
using Server.Templates;

namespace Server.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserDto>();
        CreateMap<UserEntity, UserCreatedEvent>();
        CreateMap<UserCreatedEvent, VerifyEmail>();
    }
}