using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Departments;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Departments;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/departments")]
public sealed class DepartmentsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Doctors.Create")]
    public async Task<IActionResult> Create(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateDepartmentCommand(request.Name, request.Code, request.Description);
        var result = await dispatcher.Send<CreateDepartmentCommand, Result<DepartmentDto>>(
            command,
            cancellationToken
        );
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, Map(result.Value))
            : FromError(result.Error!);
    }

    [HttpGet("{id:guid}"), HasPermission("Doctors.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<GetDepartmentByIdQuery, Result<DepartmentDto>>(
            new GetDepartmentByIdQuery(id),
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpPut("{id:guid}"), HasPermission("Doctors.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateDepartmentCommand(
            id,
            request.Name,
            request.Code,
            request.Description,
            request.HeadDoctorId
        );
        var result = await dispatcher.Send<UpdateDepartmentCommand, Result<DepartmentDto>>(
            command,
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet, HasPermission("Doctors.View")]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var result = await dispatcher.Send<SearchDepartmentsQuery, PagedResult<DepartmentDto>>(
            new SearchDepartmentsQuery(search, pageNumber, pageSize),
            cancellationToken
        );
        return Ok(
            new PagedResponse<DepartmentResponse>(
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

    [HttpDelete("{id:guid}"), HasPermission("Doctors.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<DeleteDepartmentCommand, Result>(
            new(id),
            cancellationToken
        );
        return result.IsSuccess ? NoContent() : FromError(result.Error!);
    }

    private static DepartmentResponse Map(DepartmentDto department) =>
        new(
            department.Id,
            department.Name,
            department.Code,
            department.Description,
            department.Status.ToString()
        );
}
