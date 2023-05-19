using AutoMapper;
using Server.Contracts.Events;
using Server.Templates;

namespace Server.Services;

public class EmailHandler
{
    private readonly IMapper _mapper;
    private readonly EmailService _emailService;

    public EmailHandler(IMapper mapper, EmailService emailService)
    {
        _mapper = mapper;
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
            _mapper.Map<VerifyEmail>(eventModel),
            cancellationToken);
    }
}