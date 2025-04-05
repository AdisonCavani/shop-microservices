using FluentValidation;
using Server.Contracts.Requests;

namespace Server.Validators;

public class VerifyEmailReqValidator : AbstractValidator<VerifyEmailReq>
{
    public VerifyEmailReqValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
    }
}