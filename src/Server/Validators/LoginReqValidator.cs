using FluentValidation;
using Server.Contracts.Requests;

namespace Server.Validators;

public class LoginReqValidator : AbstractValidator<LoginReq>
{
    public LoginReqValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}