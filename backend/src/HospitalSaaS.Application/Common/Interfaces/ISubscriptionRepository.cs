using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Subscriptions;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface ISubscriptionRepository
{
    Task AddAsync(TenantSubscription x, CancellationToken ct);
    Task<PagedResult<TenantSubscription>> SearchAsync(int page, int size, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
