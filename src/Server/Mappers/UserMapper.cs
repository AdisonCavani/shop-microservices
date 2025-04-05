using AutoMapper;
using ProtobufSpec.Events;
using Server.Contracts.Dtos;
using Server.Contracts.Requests;
using Server.Database.Entities;

namespace Server.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserDto>();
        CreateMap<UserEntity, ConfirmEmailEvent>().ForMember(x =>
            x.Token, map =>
                map.MapFrom((_, x) => x.Token = Guid.NewGuid()));
        CreateMap<RegisterReq, UserEntity>().ForMember(x =>
            x.Email, map =>
            map.MapFrom((c, _) => c.Email.ToLower()));
    }
}