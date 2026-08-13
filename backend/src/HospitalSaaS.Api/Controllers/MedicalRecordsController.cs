using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Clinical;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Contracts.Clinical;
using HospitalSaaS.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/medical-records")]
public sealed class MedicalRecordsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("MedicalRecords.Create")]
    public async Task<IActionResult> Create(CreateMedicalRecordRequest r, CancellationToken ct)
    {
        var c = new CreateMedicalRecordCommand(
            r.PatientId,
            r.DoctorId,
            r.AppointmentId,
            r.ChiefComplaint,
            r.History,
            r.Examination,
            r.ClinicalNotes,
            r.TreatmentPlan,
            r.FollowUpDate,
            r.Diagnoses?.Select(x => new DiagnosisInput(x.Code, x.Description, x.IsPrimary))
                .ToList()
        );
        var x = await dispatcher.Send<CreateMedicalRecordCommand, Result<MedicalRecordDto>>(c, ct);
        return x.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = x.Value!.Id }, Map(x.Value))
            : FromError(x.Error!);
    }

    [HttpPost("{id:guid}/diagnoses"), HasPermission("MedicalRecords.Create")]
    public async Task<IActionResult> AddDiagnosis(
        Guid id,
        AddDiagnosisRequest request,
        CancellationToken ct
    )
    {
        var result = await dispatcher.Send<AddDiagnosisCommand, Result<MedicalRecordDto>>(
            new(id, request.Code, request.Description, request.IsPrimary),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("{id:guid}"), HasPermission("MedicalRecords.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<GetMedicalRecordQuery, Result<MedicalRecordDto>>(new(id), ct);
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet, HasPermission("MedicalRecords.View")]
    public async Task<IActionResult> List(
        Guid? patientId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchMedicalRecordsQuery, PagedResult<MedicalRecordDto>>(
            new(patientId, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<MedicalRecordResponse>(
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

    [HttpDelete("{id:guid}"), HasPermission("MedicalRecords.Create")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<DeleteMedicalRecordCommand, Result>(new(id), ct);
        return x.IsSuccess ? NoContent() : FromError(x.Error!);
    }

    private static MedicalRecordResponse Map(MedicalRecordDto x) =>
        new(
            x.Id,
            x.PatientId,
            x.DoctorId,
            x.AppointmentId,
            x.ChiefComplaint,
            x.History,
            x.Examination,
            x.ClinicalNotes,
            x.TreatmentPlan,
            x.FollowUpDate,
            x.Diagnoses.Select(d => new DiagnosisResponse(d.Id, d.Code, d.Description, d.IsPrimary))
                .ToList()
        );
}
