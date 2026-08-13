using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Dashboard;
using HospitalSaaS.Domain.AuditLogs;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IReportingRepository
{
    Task<PagedResult<AuditLog>> SearchAuditLogsAsync(
        string? entityName,
        Guid? userId,
        int page,
        int size,
        CancellationToken ct
    );
    Task<DashboardSummaryDto> GetDashboardAsync(CancellationToken ct);
}
