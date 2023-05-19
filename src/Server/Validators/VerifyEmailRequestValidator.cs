using FluentValidation;
using Server.Contracts.Requests;

namespace Server.Validators;

public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
{
    public VerifyEmailRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
    }
}