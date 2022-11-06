using AutoMapper;
using Server.Contracts.Dtos;
using Server.Database.Entities;

namespace Server.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserDto>();
    }
}