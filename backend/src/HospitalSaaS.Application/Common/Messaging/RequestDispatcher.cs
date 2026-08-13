using HospitalSaaS.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalSaaS.Application.Common.Messaging;

public sealed class RequestDispatcher(IServiceProvider services) : IRequestDispatcher
{
    public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken ct)
        where TRequest : IRequest<TResponse>
    {
        var handler = services.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var behaviors = services
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .ToList();
        RequestHandlerDelegate<TResponse> next = token => handler.Handle(request, token);
        foreach (var behavior in behaviors)
        {
            var current = next;
            next = token => behavior.Handle(request, current, token);
        }
        return next(ct);
    }
}
