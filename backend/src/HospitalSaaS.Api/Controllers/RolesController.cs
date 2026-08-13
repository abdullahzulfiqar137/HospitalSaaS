using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Administration;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Roles;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/roles")]
public sealed class RolesController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Roles.Create")]
    public async Task<IActionResult> Create(
        CreateRoleRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await dispatcher.Send<CreateRoleCommand, Result<RoleDto>>(
            new CreateRoleCommand(request.Name, request.Permissions),
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet, HasPermission("Roles.View")]
    public async Task<IActionResult> List(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var result = await dispatcher.Send<SearchRolesQuery, PagedResult<RoleDto>>(
            new SearchRolesQuery(pageNumber, pageSize),
            cancellationToken
        );
        return Ok(
            new PagedResponse<RoleResponse>(
                result.Items.Select(Map).ToList(),
                result.PageNumber,
                result.PageSize,
                result.TotalCount,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            )
        );
    }

    private static RoleResponse Map(RoleDto role) => new(role.Id, role.Name, role.Permissions);
}
