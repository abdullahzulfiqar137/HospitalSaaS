using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Patients.Models;

namespace HospitalSaaS.Application.Patients.Commands.UpdatePatient;

public sealed class UpdatePatientCommandHandler(IPatientRepository repository)
    : IRequestHandler<UpdatePatientCommand, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(UpdatePatientCommand c, CancellationToken ct)
    {
        var p = await repository.GetAsync(c.Id, ct);
        if (p is null)
            return Result<PatientDto>.Failure(
                new("Patients.NotFound", "Patient was not found.", ErrorType.NotFound)
            );
        p.Update(
            c.FirstName,
            c.MiddleName,
            c.LastName,
            c.DateOfBirth,
            c.Gender,
            c.BloodGroup,
            c.Phone,
            c.Email,
            c.NationalId,
            c.Address,
            c.EmergencyContactName,
            c.EmergencyContactPhone,
            c.MaritalStatus,
            c.Notes
        );
        await repository.SaveChangesAsync(ct);
        return Result<PatientDto>.Success(PatientDto.From(p));
    }
}
