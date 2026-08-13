using System.Text.Json;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Domain.AuditLogs;
using HospitalSaaS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HospitalSaaS.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor(
    ICurrentUser currentUser,
    IRequestContext requestContext
) : SaveChangesInterceptor
{
    private static readonly string[] SensitiveTerms = ["Password", "Token", "Secret", "ApiKey"];

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
        var utcNow = DateTime.UtcNow;
        var entries = context
            .ChangeTracker.Entries<AuditableEntity>()
            .Where(x => x.State is EntityState.Added or EntityState.Modified)
            .ToList();
        foreach (var entry in entries)
        {
            var action = entry.Entity.IsDeleted ? "Deleted" : entry.State.ToString();
            var oldValues = entry.State == EntityState.Modified ? Serialize(entry, true) : null;
            var newValues = Serialize(entry, false);
            if (entry.State == EntityState.Added)
                entry.Entity.MarkCreated(utcNow, currentUser.UserId);
            else
                entry.Entity.MarkUpdated(utcNow, currentUser.UserId);
            Guid? tenantId = entry.Entity is TenantEntity tenantEntity
                ? tenantEntity.TenantId
                : null;
            context
                .Set<AuditLog>()
                .Add(
                    new AuditLog(
                        tenantId,
                        currentUser.UserId,
                        action,
                        entry.Metadata.ClrType.Name,
                        entry.Entity.Id.ToString(),
                        requestContext.CorrelationId,
                        oldValues,
                        newValues,
                        requestContext.IpAddress
                    )
                );
        }
    }

    private static string? Serialize(EntityEntry<AuditableEntity> entry, bool original)
    {
        var values = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            if (
                SensitiveTerms.Any(term =>
                    property.Metadata.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                )
            )
                continue;
            if (entry.State == EntityState.Modified && !property.IsModified)
                continue;
            values[property.Metadata.Name] = original
                ? property.OriginalValue
                : property.CurrentValue;
        }
        return values.Count == 0 ? null : JsonSerializer.Serialize(values);
    }
}
