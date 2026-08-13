using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Appointments;

public sealed class Appointment : AggregateRoot
{
    private Appointment() { }

    public Appointment(
        Guid tenantId,
        Guid patientId,
        Guid doctorId,
        Guid departmentId,
        DateTime dateTimeUtc,
        int durationMinutes,
        string type,
        string reason,
        string? notes
    )
        : base(tenantId)
    {
        if (dateTimeUtc.Kind != DateTimeKind.Utc)
            throw new DomainException("Appointment time must be UTC.");
        if (durationMinutes is < 5 or > 480)
            throw new DomainException("Appointment duration is invalid.");
        PatientId = patientId;
        DoctorId = doctorId;
        DepartmentId = departmentId;
        AppointmentDateTimeUtc = dateTimeUtc;
        DurationMinutes = durationMinutes;
        AppointmentType = type;
        Reason = reason;
        Notes = notes;
        Status = AppointmentStatus.Scheduled;
    }

    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public DateTime AppointmentDateTimeUtc { get; private set; }
    public int DurationMinutes { get; private set; }
    public string AppointmentType { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime EndsAtUtc => AppointmentDateTimeUtc.AddMinutes(DurationMinutes);

    public void Confirm() => Transition(AppointmentStatus.Scheduled, AppointmentStatus.Confirmed);

    public void CheckIn()
    {
        if (Status is not (AppointmentStatus.Scheduled or AppointmentStatus.Confirmed))
            Invalid();
        Status = AppointmentStatus.CheckedIn;
    }

    public void Start() => Transition(AppointmentStatus.CheckedIn, AppointmentStatus.InProgress);

    public void Complete() => Transition(AppointmentStatus.InProgress, AppointmentStatus.Completed);

    public void MarkNoShow()
    {
        if (Status is not (AppointmentStatus.Scheduled or AppointmentStatus.Confirmed))
            Invalid();
        Status = AppointmentStatus.NoShow;
    }

    public void Cancel(string reason)
    {
        if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            Invalid();
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required.");
        CancellationReason = reason;
        Status = AppointmentStatus.Cancelled;
    }

    public void Reschedule(DateTime utc, int duration)
    {
        if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            Invalid();
        if (utc.Kind != DateTimeKind.Utc)
            throw new DomainException("Appointment time must be UTC.");
        if (duration is < 5 or > 480)
            throw new DomainException("Appointment duration is invalid.");
        AppointmentDateTimeUtc = utc;
        DurationMinutes = duration;
        Status = AppointmentStatus.Scheduled;
    }

    public void Update(
        Guid doctorId,
        Guid departmentId,
        DateTime dateTimeUtc,
        int durationMinutes,
        string appointmentType,
        string reason,
        string? notes
    )
    {
        if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            Invalid();
        if (dateTimeUtc.Kind != DateTimeKind.Utc)
            throw new DomainException("Appointment time must be UTC.");
        if (durationMinutes is < 5 or > 480)
            throw new DomainException("Appointment duration is invalid.");
        if (string.IsNullOrWhiteSpace(appointmentType) || string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Appointment type and reason are required.");

        DoctorId = doctorId;
        DepartmentId = departmentId;
        AppointmentDateTimeUtc = dateTimeUtc;
        DurationMinutes = durationMinutes;
        AppointmentType = appointmentType.Trim();
        Reason = reason.Trim();
        Notes = notes?.Trim();
    }

    private void Transition(AppointmentStatus expected, AppointmentStatus next)
    {
        if (Status != expected)
            Invalid();
        Status = next;
    }

    private static void Invalid() =>
        throw new DomainException("Invalid appointment state transition.");
}

public enum AppointmentStatus
{
    Scheduled,
    Confirmed,
    CheckedIn,
    InProgress,
    Completed,
    Cancelled,
    NoShow,
}
