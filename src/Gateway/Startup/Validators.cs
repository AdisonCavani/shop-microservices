using FluentValidation;
using Gateway.Contracts.Requests;
using Gateway.Validators;

namespace Gateway.Startup;

public static class Validators
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<LoginReq>, LoginReqValidator>();
        services.AddScoped<IValidator<RegisterReq>, RegisterReqValidator>();
        services.AddScoped<IValidator<VerifyEmailReq>, VerifyEmailReqValidator>();
    }
}