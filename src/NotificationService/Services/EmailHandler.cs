using CoreShared;
using Gateway;
using ProtobufSpec.Events;
using NotificationService.Templates;

namespace NotificationService.Services;

public class EmailHandler
{
    private readonly EmailService _emailService;
    private readonly IdentityAPI.IdentityAPIClient _identityClient;

    public EmailHandler(EmailService emailService, IdentityAPI.IdentityAPIClient identityClient)
    {
        _emailService = emailService;
        _identityClient = identityClient;
    }

    public async Task VerificationMailAsync(
        ConfirmEmailEvent eventModel,
        CancellationToken cancellationToken = default)
    {
        var user = await _identityClient.GetUserAsync(new GetUserReq
        {
            Id = eventModel.UserId.ToString()
        });

        if (user is null)
            throw new Exception(ExceptionMessages.UserLost);
        
        var model = new VerifyEmail
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = eventModel.Token.ToString(),
        };
        
        await _emailService.SendEmailAsync(
            $"{user.FirstName} {user.LastName}",
            user.Email,
            VerifyEmail.Subject,
            model,
            cancellationToken);
    }

    public async Task OrderCompletedMailAsync(
        OrderCompletedEmailEvent eventModel,
        CancellationToken cancellationToken = default)
    {
        var user = await _identityClient.GetUserAsync(new GetUserReq
        {
            Id = eventModel.UserId.ToString()
        });

        if (user is null)
            throw new Exception(ExceptionMessages.UserLost);
        
        var model = new OrderCompleted
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            ActivationCode = eventModel.ActivationCode
        };
        
        await _emailService.SendEmailAsync(
            $"{user.FirstName} {user.LastName}",
            user.Email,
            VerifyEmail.Subject,
            model,
            cancellationToken);
    }
}