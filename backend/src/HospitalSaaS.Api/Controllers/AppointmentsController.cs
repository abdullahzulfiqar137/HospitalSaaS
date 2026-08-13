using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Appointments.Commands.BookAppointment;
using HospitalSaaS.Application.Appointments.Commands.ChangeAppointmentStatus;
using HospitalSaaS.Application.Appointments.Commands.UpdateAppointment;
using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Appointments.Queries.GetAppointmentById;
using HospitalSaaS.Application.Appointments.Queries.SearchAppointments;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Contracts.Appointments;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Domain.Appointments;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/appointments")]
public sealed class AppointmentsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Appointments.Create")]
    public async Task<IActionResult> Book(
        BookAppointmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new BookAppointmentCommand(
            request.PatientId,
            request.DoctorId,
            request.DepartmentId,
            request.AppointmentDateTimeUtc,
            request.DurationMinutes,
            request.AppointmentType,
            request.Reason,
            request.Notes
        );
        var result = await dispatcher.Send<BookAppointmentCommand, Result<AppointmentDto>>(
            command,
            cancellationToken
        );
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, Map(result.Value))
            : FromError(result.Error!);
    }

    [HttpGet("{id:guid}"), HasPermission("Appointments.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<GetAppointmentByIdQuery, Result<AppointmentDto>>(
            new GetAppointmentByIdQuery(id),
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpPut("{id:guid}"), HasPermission("Appointments.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateAppointmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await dispatcher.Send<UpdateAppointmentCommand, Result<AppointmentDto>>(
            new(
                id,
                request.DoctorId,
                request.DepartmentId,
                request.AppointmentDateTimeUtc,
                request.DurationMinutes,
                request.AppointmentType,
                request.Reason,
                request.Notes
            ),
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet, HasPermission("Appointments.View")]
    public async Task<IActionResult> List(
        [FromQuery] DateTime? date,
        [FromQuery] Guid? doctorId,
        [FromQuery] Guid? patientId,
        [FromQuery] AppointmentStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var query = new SearchAppointmentsQuery(
            date,
            doctorId,
            patientId,
            status,
            pageNumber,
            pageSize
        );
        var result = await dispatcher.Send<SearchAppointmentsQuery, PagedResult<AppointmentDto>>(
            query,
            cancellationToken
        );
        return Ok(
            new PagedResponse<AppointmentResponse>(
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

    [HttpPost("{id:guid}/actions"), HasPermission("Appointments.Update")]
    public async Task<IActionResult> Action(
        Guid id,
        AppointmentActionRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!Enum.TryParse<AppointmentAction>(request.Action, true, out var action))
            return FromError(
                new Error(
                    "Appointments.Action",
                    "Unknown appointment action.",
                    ErrorType.Validation
                )
            );

        var command = new ChangeAppointmentStatusCommand(
            id,
            action,
            request.Reason,
            request.AppointmentDateTimeUtc,
            request.DurationMinutes
        );
        var result = await dispatcher.Send<ChangeAppointmentStatusCommand, Result<AppointmentDto>>(
            command,
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    private static AppointmentResponse Map(AppointmentDto appointment) =>
        new(
            appointment.Id,
            appointment.PatientId,
            appointment.DoctorId,
            appointment.DepartmentId,
            appointment.AppointmentDateTimeUtc,
            appointment.DurationMinutes,
            appointment.AppointmentType,
            appointment.Status.ToString(),
            appointment.Reason,
            appointment.Notes
        );
}
