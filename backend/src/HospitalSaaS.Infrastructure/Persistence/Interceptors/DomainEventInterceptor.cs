using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HospitalSaaS.Infrastructure.Persistence.Interceptors;

public sealed class DomainEventInterceptor(IDomainEventDispatcher dispatcher)
    : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not null)
        {
            var roots = eventData
                .Context.ChangeTracker.Entries<AggregateRoot>()
                .Select(x => x.Entity)
                .Where(x => x.DomainEvents.Count > 0)
                .ToList();
            var events = roots.SelectMany(x => x.DomainEvents).ToList();
            foreach (var root in roots)
                root.ClearDomainEvents();
            if (events.Count > 0)
                await dispatcher.DispatchAsync(events, cancellationToken);
        }
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
