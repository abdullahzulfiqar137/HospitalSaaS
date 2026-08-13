using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Clinical;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Contracts.Clinical;
using HospitalSaaS.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/prescriptions")]
public sealed class PrescriptionsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Prescriptions.Create")]
    public async Task<IActionResult> Create(CreatePrescriptionRequest r, CancellationToken ct)
    {
        var c = new CreatePrescriptionCommand(
            r.PatientId,
            r.DoctorId,
            r.AppointmentId,
            r.Notes,
            r.Items.Select(i => new PrescriptionItemInput(
                    i.MedicineId,
                    i.Dose,
                    i.Frequency,
                    i.Duration,
                    i.Route,
                    i.Instructions
                ))
                .ToList()
        );
        var x = await dispatcher.Send<CreatePrescriptionCommand, Result<PrescriptionDto>>(c, ct);
        return x.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = x.Value!.Id }, Map(x.Value))
            : FromError(x.Error!);
    }

    [HttpGet("{id:guid}"), HasPermission("Prescriptions.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<GetPrescriptionQuery, Result<PrescriptionDto>>(new(id), ct);
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet, HasPermission("Prescriptions.View")]
    public async Task<IActionResult> List(
        Guid? patientId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchPrescriptionsQuery, PagedResult<PrescriptionDto>>(
            new(patientId, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<PrescriptionResponse>(
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

    private static PrescriptionResponse Map(PrescriptionDto x) =>
        new(
            x.Id,
            x.PatientId,
            x.DoctorId,
            x.AppointmentId,
            x.DateUtc,
            x.Notes,
            x.Items.Select(i => new PrescriptionLineResponse(
                    i.Id,
                    i.MedicineId,
                    i.Dose,
                    i.Frequency,
                    i.Duration,
                    i.Route,
                    i.Instructions
                ))
                .ToList()
        );
}
