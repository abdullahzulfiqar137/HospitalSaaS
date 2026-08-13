using FluentValidation;

namespace HospitalSaaS.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct
    )
    {
        var validatorList = validators?.Where(v => v is not null).ToList();
        if (validatorList is null || validatorList.Count == 0)
        {
            return await next(ct);
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validatorList.Select(x => x.ValidateAsync(context, CancellationToken.None))
        );

        var errors = validationResults
            .Where(r => r is not null && r.Errors is not null)
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (errors.Count > 0)
            throw new ValidationException(errors);

        return await next(ct);
    }
}
