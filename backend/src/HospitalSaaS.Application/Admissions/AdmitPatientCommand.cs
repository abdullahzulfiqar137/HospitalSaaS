using FluentValidation;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Admissions;

public sealed record AdmitPatientCommand(
    Guid PatientId,
    Guid DoctorId,
    string AdmissionType,
    string Reason,
    Guid WardId,
    Guid RoomId,
    Guid BedId
) : ICommand<Result<AdmissionDto>>;

public sealed class AdmitPatientCommandValidator : AbstractValidator<AdmitPatientCommand>
{
    public AdmitPatientCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.WardId).NotEmpty();
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.BedId).NotEmpty();
        RuleFor(x => x.AdmissionType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
