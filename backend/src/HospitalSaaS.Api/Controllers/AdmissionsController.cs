using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Admissions;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Contracts.Admissions;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/admissions")]
public sealed class AdmissionsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost("wards"), HasPermission("Admissions.Create")]
    public async Task<IActionResult> Ward(CreateWardRequest request, CancellationToken ct)
    {
        var result = await dispatcher.Send<CreateWardCommand, Result<WardDto>>(
            new(request.Name, request.Code),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpPost("rooms"), HasPermission("Admissions.Create")]
    public async Task<IActionResult> Room(CreateRoomRequest request, CancellationToken ct)
    {
        var result = await dispatcher.Send<CreateRoomCommand, Result<RoomDto>>(
            new(request.WardId, request.Number),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpPost("beds"), HasPermission("Admissions.Create")]
    public async Task<IActionResult> Bed(CreateBedRequest request, CancellationToken ct)
    {
        var result = await dispatcher.Send<CreateBedCommand, Result<BedDto>>(
            new(request.RoomId, request.Number),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpPost, HasPermission("Admissions.Create")]
    public async Task<IActionResult> Admit(AdmitPatientRequest request, CancellationToken ct)
    {
        var command = new AdmitPatientCommand(
            request.PatientId,
            request.DoctorId,
            request.AdmissionType,
            request.Reason,
            request.WardId,
            request.RoomId,
            request.BedId
        );
        var result = await dispatcher.Send<AdmitPatientCommand, Result<AdmissionDto>>(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, Map(result.Value))
            : FromError(result.Error!);
    }

    [HttpGet("{id:guid}"), HasPermission("Admissions.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.Send<GetAdmissionByIdQuery, Result<AdmissionDto>>(
            new(id),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpPost("{id:guid}/transfer"), HasPermission("Admissions.Transfer")]
    public async Task<IActionResult> Transfer(
        Guid id,
        TransferAdmissionRequest request,
        CancellationToken ct
    )
    {
        var result = await dispatcher.Send<TransferAdmissionCommand, Result<AdmissionDto>>(
            new(id, request.WardId, request.RoomId, request.BedId, request.Reason),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("{id:guid}/history"), HasPermission("Admissions.View")]
    public async Task<IActionResult> History(Guid id, CancellationToken ct)
    {
        var history = await dispatcher.Send<
            GetAdmissionHistoryQuery,
            IReadOnlyList<AdmissionHistoryDto>
        >(new(id), ct);
        return Ok(
            history.Select(x => new AdmissionHistoryResponse(
                x.Id,
                x.AdmissionId,
                x.FromWardId,
                x.FromRoomId,
                x.FromBedId,
                x.ToWardId,
                x.ToRoomId,
                x.ToBedId,
                x.Reason,
                x.TransferredAtUtc
            ))
        );
    }

    [HttpPost("{id:guid}/discharge"), HasPermission("Admissions.Discharge")]
    public async Task<IActionResult> Discharge(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.Send<DischargeAdmissionCommand, Result>(new(id), ct);
        return result.IsSuccess ? NoContent() : FromError(result.Error!);
    }

    private static WardResponse Map(WardDto x) => new(x.Id, x.Name, x.Code, x.IsActive);

    private static RoomResponse Map(RoomDto x) => new(x.Id, x.WardId, x.Number, x.IsActive);

    private static BedResponse Map(BedDto x) => new(x.Id, x.RoomId, x.Number, x.IsOccupied);

    private static AdmissionResponse Map(AdmissionDto x) =>
        new(
            x.Id,
            x.AdmissionNumber,
            x.PatientId,
            x.DoctorId,
            x.AdmissionDateUtc,
            x.AdmissionType,
            x.Reason,
            x.WardId,
            x.RoomId,
            x.BedId,
            x.Status.ToString(),
            x.DischargeDateUtc
        );
}
