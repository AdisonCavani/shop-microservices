using AutoMapper;
using Gateway.Contracts.Dtos;
using Gateway.Contracts.Requests;
using Gateway.Database.Entities;
using ProtobufSpec.Events;

namespace Gateway.Mappers;

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