using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;

namespace HospitalSaaS.Application.Dashboard;

public sealed record DashboardSummaryDto(
    int Patients,
    int Doctors,
    int AppointmentsToday,
    int ActiveAdmissions,
    int UnpaidInvoices,
    int PendingLabOrders,
    int LowStockMedicines,
    int PendingNotifications
);

public sealed record GetDashboardSummaryQuery : IQuery<DashboardSummaryDto>;

public sealed class GetDashboardSummaryQueryHandler(IReportingRepository r)
    : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery x, CancellationToken ct) =>
        r.GetDashboardAsync(ct);
}
