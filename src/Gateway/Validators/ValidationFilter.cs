using FluentValidation;

namespace Gateway.Validators;

public class ValidationFilter<TRequest> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var loginReq = context.GetArgument<TRequest>(0);
        var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<TRequest>>();
        var validationResult = await validator.ValidateAsync(loginReq);
        
        if (!validationResult.IsValid) 
            return Results.ValidationProblem(validationResult.ToDictionary());

        return await next(context);
    }
}