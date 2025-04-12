using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductService.Contracts.Requests;
using ProductService.Database;

namespace ProductService.Validators;

public class CreateProductReqValidator : AbstractValidator<CreateProductReq>
{
    public CreateProductReqValidator(AppDbContext dbContext)
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.PriceCents).GreaterThan(200);
        RuleFor(x => x.ActivationCode)
            .NotEmpty()
            .CustomAsync(async (value, context, cancellationToken) =>
            {
                var taken = await dbContext.Products.FirstOrDefaultAsync(x => x.ActivationCode == value, cancellationToken);

                if (taken is not null)
                    context.AddFailure(nameof(context.PropertyName), "Code already exists");
            });
    }
}