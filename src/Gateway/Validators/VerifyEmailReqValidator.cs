using FluentValidation;
using Gateway.Contracts.Requests;

namespace Gateway.Validators;

public class VerifyEmailReqValidator : AbstractValidator<VerifyEmailReq>
{
    public VerifyEmailReqValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
    }
}