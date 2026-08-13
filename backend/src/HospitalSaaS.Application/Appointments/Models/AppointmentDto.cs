using HospitalSaaS.Domain.Appointments;

namespace HospitalSaaS.Application.Appointments.Models;

public sealed record AppointmentDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    Guid DepartmentId,
    DateTime AppointmentDateTimeUtc,
    int DurationMinutes,
    string AppointmentType,
    AppointmentStatus Status,
    string Reason,
    string? Notes,
    string? CancellationReason
)
{
    public static AppointmentDto From(Appointment appointment) =>
        new(
            appointment.Id,
            appointment.PatientId,
            appointment.DoctorId,
            appointment.DepartmentId,
            appointment.AppointmentDateTimeUtc,
            appointment.DurationMinutes,
            appointment.AppointmentType,
            appointment.Status,
            appointment.Reason,
            appointment.Notes,
            appointment.CancellationReason
        );
}
