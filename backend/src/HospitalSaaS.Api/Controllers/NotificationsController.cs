using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Notifications;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Support;
using HospitalSaaS.Domain.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/notifications")]
public sealed class NotificationsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Users.Update")]
    public async Task<IActionResult> Create(CreateNotificationRequest r, CancellationToken ct)
    {
        var x = await dispatcher.Send<CreateNotificationCommand, Result<NotificationDto>>(
            new(r.UserId, r.Subject, r.Body, r.Channel),
            ct
        );
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet, HasPermission("Users.View")]
    public async Task<IActionResult> List(
        Guid? userId,
        NotificationStatus? status,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchNotificationsQuery, PagedResult<NotificationDto>>(
            new(userId, status, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<NotificationResponse>(
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

    private static NotificationResponse Map(NotificationDto x) =>
        new(x.Id, x.UserId, x.Subject, x.Channel, x.Status.ToString(), x.SentAtUtc);
}
