using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Dashboard;
using HospitalSaaS.Contracts.Support;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/dashboard")]
public sealed class DashboardController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpGet, HasPermission("Dashboard.View")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var x = await dispatcher.Send<GetDashboardSummaryQuery, DashboardSummaryDto>(new(), ct);
        return Ok(
            new DashboardResponse(
                x.Patients,
                x.Doctors,
                x.AppointmentsToday,
                x.ActiveAdmissions,
                x.UnpaidInvoices,
                x.PendingLabOrders,
                x.LowStockMedicines,
                x.PendingNotifications
            )
        );
    }
}
