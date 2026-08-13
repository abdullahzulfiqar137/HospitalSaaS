using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Doctors.Commands.CreateDoctor;
using HospitalSaaS.Application.Doctors.Commands.DeleteDoctor;
using HospitalSaaS.Application.Doctors.Commands.SetDoctorStatus;
using HospitalSaaS.Application.Doctors.Commands.UpdateDoctor;
using HospitalSaaS.Application.Doctors.Models;
using HospitalSaaS.Application.Doctors.Queries.GetDoctorById;
using HospitalSaaS.Application.Doctors.Queries.SearchDoctors;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Doctors;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/doctors")]
public sealed class DoctorsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Doctors.Create")]
    public async Task<IActionResult> Create(
        CreateDoctorRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateDoctorCommand(
            request.EmployeeNumber,
            request.FirstName,
            request.LastName,
            request.Specialization,
            request.DepartmentId,
            request.LicenseNumber,
            request.Phone,
            request.Email,
            request.ConsultationFee
        );
        var result = await dispatcher.Send<CreateDoctorCommand, Result<DoctorDto>>(
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
        var result = await dispatcher.Send<GetDoctorByIdQuery, Result<DoctorDto>>(
            new GetDoctorByIdQuery(id),
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpPut("{id:guid}"), HasPermission("Doctors.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateDoctorRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateDoctorCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Specialization,
            request.DepartmentId,
            request.LicenseNumber,
            request.Phone,
            request.Email,
            request.ConsultationFee
        );
        var result = await dispatcher.Send<UpdateDoctorCommand, Result<DoctorDto>>(
            command,
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet, HasPermission("Doctors.View")]
    public async Task<IActionResult> Search(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var result = await dispatcher.Send<SearchDoctorsQuery, PagedResult<DoctorDto>>(
            new SearchDoctorsQuery(search, pageNumber, pageSize),
            cancellationToken
        );
        return Ok(
            new PagedResponse<DoctorResponse>(
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

    [HttpPost("{id:guid}/activate"), HasPermission("Doctors.Update")]
    public Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken) =>
        SetStatus(id, true, cancellationToken);

    [HttpPost("{id:guid}/deactivate"), HasPermission("Doctors.Update")]
    public Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken) =>
        SetStatus(id, false, cancellationToken);

    [HttpDelete("{id:guid}"), HasPermission("Doctors.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<DeleteDoctorCommand, Result>(
            new DeleteDoctorCommand(id),
            cancellationToken
        );
        return result.IsSuccess ? NoContent() : FromError(result.Error!);
    }

    private async Task<IActionResult> SetStatus(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken
    )
    {
        var result = await dispatcher.Send<SetDoctorStatusCommand, Result>(
            new SetDoctorStatusCommand(id, isActive),
            cancellationToken
        );
        return result.IsSuccess ? NoContent() : FromError(result.Error!);
    }

    private static DoctorResponse Map(DoctorDto doctor) =>
        new(
            doctor.Id,
            doctor.EmployeeNumber,
            doctor.FirstName,
            doctor.LastName,
            doctor.Specialization,
            doctor.DepartmentId,
            doctor.LicenseNumber,
            doctor.Phone,
            doctor.Email,
            doctor.ConsultationFee,
            doctor.Status.ToString()
        );
}
