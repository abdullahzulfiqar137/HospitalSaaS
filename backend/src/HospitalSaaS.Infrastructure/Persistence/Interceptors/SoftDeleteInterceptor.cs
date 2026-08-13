using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HospitalSaaS.Infrastructure.Persistence.Interceptors;

public sealed class SoftDeleteInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null)
            return;
        foreach (
            var entry in context
                .ChangeTracker.Entries<AuditableEntity>()
                .Where(x => x.State == EntityState.Deleted)
        )
        {
            entry.State = EntityState.Modified;
            entry.Entity.SoftDelete(DateTime.UtcNow, currentUser.UserId);
        }
    }
}
