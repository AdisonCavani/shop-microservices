using AutoMapper;
using Server.Contracts.Dtos;
using Server.Contracts.Events;
using Server.Contracts.Requests;
using Server.Database.Entities;
using Server.Templates;

namespace Server.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserDto>();
        CreateMap<UserEntity, ConfirmEmailEvent>().ForMember(x =>
            x.Token, map =>
                map.MapFrom((_, x) => x.Token = Guid.NewGuid()));
        CreateMap<ConfirmEmailEvent, VerifyEmail>();
        CreateMap<RegisterRequest, UserEntity>().ForMember(x =>
            x.Email, map =>
            map.MapFrom((c, _) => c.Email.ToLower()));
    }
}