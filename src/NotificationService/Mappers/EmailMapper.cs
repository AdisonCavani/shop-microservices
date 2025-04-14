using NotificationService.Templates;
using ProtobufSpec.Events;

namespace NotificationService.Mappers;

public static class EmailMapper
{
    public static VerifyEmail ToVerifyEmail (this ConfirmEmailEvent confirmEmailEvent) {
        return new VerifyEmail
        {
            Token = confirmEmailEvent.Token.ToString(),
            FirstName = confirmEmailEvent.FirstName,
            LastName = confirmEmailEvent.LastName
        };
    }
}