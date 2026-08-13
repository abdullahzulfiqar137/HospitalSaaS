using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Patients.Commands.CreatePatient;
using HospitalSaaS.Application.Patients.Commands.DeletePatient;
using HospitalSaaS.Application.Patients.Commands.UpdatePatient;
using HospitalSaaS.Application.Patients.Models;
using HospitalSaaS.Application.Patients.Queries.GetPatientById;
using HospitalSaaS.Application.Patients.Queries.SearchPatients;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Patients;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/patients")]
public sealed class PatientsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Patients.Create")]
    public async Task<IActionResult> Create(
        CreatePatientRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CreatePatientCommand(
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.Phone,
            request.Email,
            request.NationalId,
            request.Address,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.MaritalStatus,
            request.Notes
        );
        var result = await dispatcher.Send<CreatePatientCommand, Result<PatientDto>>(
            command,
            cancellationToken
        );
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, Map(result.Value))
            : FromError(result.Error!);
    }

    [HttpPut("{id:guid}"), HasPermission("Patients.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdatePatientRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdatePatientCommand(
            id,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.BloodGroup,
            request.Phone,
            request.Email,
            request.NationalId,
            request.Address,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.MaritalStatus,
            request.Notes
        );
        var result = await dispatcher.Send<UpdatePatientCommand, Result<PatientDto>>(
            command,
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("{id:guid}"), HasPermission("Patients.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<GetPatientByIdQuery, Result<PatientDto>>(
            new GetPatientByIdQuery(id),
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet, HasPermission("Patients.View")]
    public async Task<IActionResult> Search(
        [FromQuery] string? query,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortDirection = "desc",
        CancellationToken cancellationToken = default
    )
    {
        var request = new SearchPatientsQuery(
            query,
            pageNumber,
            pageSize,
            sortBy,
            sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
        );
        var result = await dispatcher.Send<SearchPatientsQuery, PagedResult<PatientDto>>(
            request,
            cancellationToken
        );
        return Ok(
            new PagedResponse<PatientResponse>(
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

    [HttpDelete("{id:guid}"), HasPermission("Patients.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<DeletePatientCommand, Result>(
            new DeletePatientCommand(id),
            cancellationToken
        );
        return result.IsSuccess ? NoContent() : FromError(result.Error!);
    }

    private static PatientResponse Map(PatientDto patient) =>
        new(
            patient.Id,
            patient.PatientNumber,
            patient.FirstName,
            patient.MiddleName,
            patient.LastName,
            patient.DateOfBirth,
            patient.Gender,
            patient.BloodGroup,
            patient.Phone,
            patient.Email,
            patient.NationalId,
            patient.Address,
            patient.EmergencyContactName,
            patient.EmergencyContactPhone,
            patient.MaritalStatus,
            patient.RegistrationDateUtc,
            patient.Notes,
            patient.Status.ToString()
        );
}
