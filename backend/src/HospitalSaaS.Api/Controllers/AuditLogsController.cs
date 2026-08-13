using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.AuditLogs;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Support;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/audit-logs")]
public sealed class AuditLogsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpGet, HasPermission("Roles.View")]
    public async Task<IActionResult> List(
        string? entityName,
        Guid? userId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchAuditLogsQuery, PagedResult<AuditLogDto>>(
            new(entityName, userId, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<AuditLogResponse>(
                x.Items.Select(Map).ToList(),
                x.PageNumber,
                x.PageSize,
                x.TotalCount,
                x.TotalPages,
                x.HasPreviousPage,
                x.HasNextPage
            )
        );
    }

    private static AuditLogResponse Map(AuditLogDto x) =>
        new(
            x.Id,
            x.UserId,
            x.Action,
            x.EntityName,
            x.EntityId,
            x.CorrelationId,
            x.OldValues,
            x.NewValues,
            x.IpAddress,
            x.OccurredAtUtc
        );
}
