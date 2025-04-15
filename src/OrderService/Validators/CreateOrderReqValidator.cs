using FluentValidation;
using OrderService.Contracts.Requests;

namespace OrderService.Validators;

public class CreateOrderReqValidator : AbstractValidator<CreateOrderReq>
{
    public CreateOrderReqValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}