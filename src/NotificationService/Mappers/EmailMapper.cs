using AutoMapper;
using NotificationService.Templates;
using ProtobufSpec.Events;

namespace NotificationService.Mappers;

public class EmailMapper : Profile
{
    public EmailMapper()
    {
        CreateMap<ConfirmEmailEvent, VerifyEmail>();
    }
}