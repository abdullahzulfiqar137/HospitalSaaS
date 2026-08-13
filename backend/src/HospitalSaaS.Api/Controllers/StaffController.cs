using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Staff;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Support;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/staff")]
public sealed class StaffController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Users.Create")]
    public async Task<IActionResult> Create(CreateStaffRequest r, CancellationToken ct)
    {
        var x = await dispatcher.Send<CreateStaffCommand, Result<StaffDto>>(
            new(r.EmployeeNumber, r.FirstName, r.LastName, r.JobTitle, r.DepartmentId),
            ct
        );
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet, HasPermission("Users.View")]
    public async Task<IActionResult> List(
        string? search,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchStaffQuery, PagedResult<StaffDto>>(
            new(search, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<StaffResponse>(
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

    private static StaffResponse Map(StaffDto x) =>
        new(
            x.Id,
            x.EmployeeNumber,
            x.FirstName,
            x.LastName,
            x.JobTitle,
            x.DepartmentId,
            x.IsActive
        );
}
