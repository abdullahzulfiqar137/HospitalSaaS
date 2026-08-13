using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class SubscriptionRepository(HospitalDbContext db) : ISubscriptionRepository
{
    public Task AddAsync(TenantSubscription x, CancellationToken ct) =>
        db.TenantSubscriptions.AddAsync(x, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<TenantSubscription>> SearchAsync(
        int page,
        int size,
        CancellationToken ct
    )
    {
        var q = db.TenantSubscriptions.AsNoTracking();
        var c = await q.CountAsync(ct);
        return new(
            await q.OrderByDescending(x => x.StartsAtUtc)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct),
            page,
            size,
            c
        );
    }
}
