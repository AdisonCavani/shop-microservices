using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Server.Contracts.Requests;
using Server.Database;
using Server.Resolvers;

namespace Server.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(AppDbContext dbContext)
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
                    context.AddFailure("Email", ExceptionMessages.EmailTaken);
            });

        // TODO: add password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}