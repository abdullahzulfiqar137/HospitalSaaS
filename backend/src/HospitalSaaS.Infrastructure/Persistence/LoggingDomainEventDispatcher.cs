using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Domain.Common;
using Microsoft.Extensions.Logging;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class LoggingDomainEventDispatcher(ILogger<LoggingDomainEventDispatcher> logger)
    : IDomainEventDispatcher
{
    public Task DispatchAsync(
        IReadOnlyCollection<DomainEvent> events,
        CancellationToken cancellationToken
    )
    {
        foreach (var domainEvent in events)
            logger.LogInformation(
                "Dispatched domain event {DomainEventType} {DomainEventId}",
                domainEvent.GetType().Name,
                domainEvent.Id
            );
        return Task.CompletedTask;
    }
}
