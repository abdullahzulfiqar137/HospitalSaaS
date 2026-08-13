using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HospitalSaaS.Api.Filters;

public sealed class ValidationFilter(IServiceProvider services) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var failures = new List<FluentValidation.Results.ValidationFailure>();
        foreach (var argument in context.ActionArguments.Values.Where(x => x is not null))
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
            if (services.GetService(validatorType) is not IValidator validator)
                continue;
            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted
            );
            failures.AddRange(result.Errors);
        }
        if (failures.Count == 0)
        {
            await next();
            return;
        }
        context.Result = new BadRequestObjectResult(
            new
            {
                code = "Validation.Failed",
                message = "One or more validation errors occurred.",
                correlationId = context.HttpContext.TraceIdentifier,
                errors = failures
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).ToArray()),
            }
        );
    }
}
