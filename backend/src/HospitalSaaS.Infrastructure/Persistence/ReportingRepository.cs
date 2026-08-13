using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Dashboard;
using HospitalSaaS.Domain.Admissions;
using HospitalSaaS.Domain.AuditLogs;
using HospitalSaaS.Domain.Billing;
using HospitalSaaS.Domain.Laboratory;
using HospitalSaaS.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class ReportingRepository(HospitalDbContext db, ITenantContext tenant)
    : IReportingRepository
{
    public async Task<PagedResult<AuditLog>> SearchAuditLogsAsync(
        string? entity,
        Guid? user,
        int page,
        int size,
        CancellationToken ct
    )
    {
        var q = db.AuditLogs.AsNoTracking().Where(x => x.TenantId == tenant.TenantId);
        if (!string.IsNullOrWhiteSpace(entity))
            q = q.Where(x => x.EntityName == entity);
        if (user.HasValue)
            q = q.Where(x => x.UserId == user);
        var c = await q.CountAsync(ct);
        return new(
            await q.OrderByDescending(x => x.OccurredAtUtc)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct),
            page,
            size,
            c
        );
    }

    public async Task<DashboardSummaryDto> GetDashboardAsync(CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var low = await db.Medicines.CountAsync(
            m =>
                (
                    db.StockTransactions.Where(t => t.MedicineId == m.Id).Sum(t => (int?)t.Quantity)
                    ?? 0
                ) <= m.MinimumStockLevel,
            ct
        );
        return new(
            await db.Patients.CountAsync(ct),
            await db.Doctors.CountAsync(ct),
            await db.Appointments.CountAsync(
                x =>
                    x.AppointmentDateTimeUtc >= today
                    && x.AppointmentDateTimeUtc < today.AddDays(1),
                ct
            ),
            await db.Admissions.CountAsync(x => x.Status == AdmissionStatus.Admitted, ct),
            await db.Invoices.CountAsync(
                x => x.Status != InvoiceStatus.Paid && x.Status != InvoiceStatus.Cancelled,
                ct
            ),
            await db.LabOrders.CountAsync(
                x => x.Status != LabOrderStatus.Completed && x.Status != LabOrderStatus.Cancelled,
                ct
            ),
            low,
            await db.Notifications.CountAsync(x => x.Status == NotificationStatus.Pending, ct)
        );
    }
}
