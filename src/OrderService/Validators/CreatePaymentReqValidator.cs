using FluentValidation;
using OrderService.Contracts.Requests;

namespace OrderService.Validators;

public class CreatePaymentReqValidator : AbstractValidator<CreatePaymentReq>
{
    public CreatePaymentReqValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}