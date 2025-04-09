using NotificationService.Mappers;
using ProtobufSpec.Events;
using NotificationService.Templates;

namespace NotificationService.Services;

public class EmailHandler
{
    private readonly EmailService _emailService;

    public EmailHandler(EmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task<bool> VerificationMailAsync(
        ConfirmEmailEvent eventModel,
        CancellationToken cancellationToken = default)
    {
        return await _emailService.SendEmailAsync(
            $"{eventModel.FirstName} {eventModel.LastName}",
            eventModel.Email,
            VerifyEmail.Subject,
            eventModel.ToVerifyEmail(),
            cancellationToken);
    }
}