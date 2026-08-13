using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Subscriptions;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Support;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/subscriptions")]
public sealed class SubscriptionsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Tenants.Update")]
    public async Task<IActionResult> Create(CreateSubscriptionRequest r, CancellationToken ct)
    {
        var x = await dispatcher.Send<CreateSubscriptionCommand, Result<SubscriptionDto>>(
            new(r.Plan, r.StartsAtUtc, r.EndsAtUtc),
            ct
        );
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet, HasPermission("Tenants.View")]
    public async Task<IActionResult> List(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchSubscriptionsQuery, PagedResult<SubscriptionDto>>(
            new(pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<SubscriptionResponse>(
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

    private static SubscriptionResponse Map(SubscriptionDto x) =>
        new(x.Id, x.Plan, x.StartsAtUtc, x.EndsAtUtc, x.Status.ToString());
}
