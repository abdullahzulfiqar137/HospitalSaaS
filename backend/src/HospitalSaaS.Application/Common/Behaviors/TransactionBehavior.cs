using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;

namespace HospitalSaaS.Application.Common.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unit)
    : IPipelineBehavior<TRequest, TResponse>
{
    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct
    ) =>
        request is ICommand<TResponse>
            ? unit.ExecuteInTransactionAsync<TResponse>(token => next(token), ct)
            : next(ct);
}
