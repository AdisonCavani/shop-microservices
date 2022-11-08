using Server.Templates;

namespace Server.Services;

public class EmailHandler
{
    private readonly EmailService _emailService;

    public EmailHandler(EmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task<bool> VerificationMailAsync(
        string name,
        string email,
        string token,
        CancellationToken cancellationToken = default)
    {
        var model = new VerifyEmail
        {
            Name = name,
            Token = token
        };

        return await _emailService.SendEmailAsync(
            name,
            email,
            VerifyEmail.Subject,
            model,
            cancellationToken);
    }
}