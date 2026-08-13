using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Notifications;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification x, CancellationToken ct);
    Task<PagedResult<Notification>> SearchAsync(
        Guid? userId,
        NotificationStatus? status,
        int page,
        int size,
        CancellationToken ct
    );
    Task SaveChangesAsync(CancellationToken ct);
}
