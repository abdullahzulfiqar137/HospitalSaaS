using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IReadOnlyCollection<DomainEvent> events,
        CancellationToken cancellationToken
    );
}
