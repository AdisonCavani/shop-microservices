using CoreShared;
using FluentValidation;
using Gateway.Contracts.Requests;
using Gateway.Database;
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
                var taken = await dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Email == value.ToLower(), cancellationToken);

                if (taken is not null)
                    context.AddFailure(nameof(context.PropertyPath), ExceptionMessages.EmailTaken);
            });
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}