using FluentValidation;

namespace HospitalSaaS.Application.Appointments.Commands.BookAppointment;

public sealed class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.AppointmentDateTimeUtc)
            .Must(x => x.Kind == DateTimeKind.Utc)
            .WithMessage("Appointment time must be UTC.");
        RuleFor(x => x.DurationMinutes).InclusiveBetween(5, 480);
        RuleFor(x => x.AppointmentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Notes).MaximumLength(4000);
    }
}
