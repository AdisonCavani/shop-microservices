using FluentValidation;
using Gateway.Contracts.Requests;
using Gateway.Database;
using Gateway.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Validators;

public class RegisterReqValidator : AbstractValidator<RegisterReq>
{
    public RegisterReqValidator(AppDbContext dbContext)
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .CustomAsync(async (value, context, cancellationToken) =>
            {
                var taken = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == value.ToLower(), cancellationToken);

                if (taken is not null)
                    context.AddFailure(nameof(context.PropertyName), ExceptionMessages.EmailTaken);
            });
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}