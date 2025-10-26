using FluentValidation;
using NotificationService.Contracts.Requests;

namespace NotificationService.Validators;

public class CreateNotificationTriggerReqValidator : AbstractValidator<CreateNotificationTriggerReq>
{
    public CreateNotificationTriggerReqValidator()
    {
        RuleFor(x => x.TriggerName).NotEmpty();
        RuleFor(x => x.LiquidTemplate).NotEmpty();
    }
}