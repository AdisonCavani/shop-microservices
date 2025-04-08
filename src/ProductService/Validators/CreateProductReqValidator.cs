using FluentValidation;
using ProductService.Contracts.Requests;

namespace ProductService.Validators;

public class CreateProductReqValidator : AbstractValidator<CreateProductReq>
{
    public CreateProductReqValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}