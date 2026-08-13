using FluentValidation;

namespace HospitalSaaS.Application.Appointments.Commands.ChangeAppointmentStatus;

public sealed class ChangeAppointmentStatusCommandValidator
    : AbstractValidator<ChangeAppointmentStatusCommand>
{
    public ChangeAppointmentStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().When(x => x.Action == AppointmentAction.Cancel);
        RuleFor(x => x.AppointmentDateTimeUtc)
            .NotNull()
            .When(x => x.Action == AppointmentAction.Reschedule);
        RuleFor(x => x.AppointmentDateTimeUtc!.Value)
            .Must(x => x.Kind == DateTimeKind.Utc)
            .When(x => x.AppointmentDateTimeUtc.HasValue)
            .WithMessage("Appointment time must be UTC.");
        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(5, 480)
            .When(x => x.DurationMinutes.HasValue);
    }
}
