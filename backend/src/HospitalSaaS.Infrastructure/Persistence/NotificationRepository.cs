using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class NotificationRepository(HospitalDbContext db) : INotificationRepository
{
    public Task AddAsync(Notification x, CancellationToken ct) =>
        db.Notifications.AddAsync(x, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<Notification>> SearchAsync(
        Guid? userId,
        NotificationStatus? status,
        int page,
        int size,
        CancellationToken ct
    )
    {
        var q = db.Notifications.AsNoTracking();
        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId);
        if (status.HasValue)
            q = q.Where(x => x.Status == status);
        var c = await q.CountAsync(ct);
        return new(
            await q.OrderByDescending(x => x.CreatedAtUtc)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct),
            page,
            size,
            c
        );
    }
}
