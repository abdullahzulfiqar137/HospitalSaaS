using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Patients.Models;
using HospitalSaaS.Domain.Patients;

namespace HospitalSaaS.Application.Patients.Commands.CreatePatient;

public sealed class CreatePatientCommandHandler(
    IPatientRepository repository,
    ITenantContext tenant
) : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(CreatePatientCommand command, CancellationToken ct)
    {
        var sequence = await repository.CountRegisteredInYearAsync(DateTime.UtcNow.Year, ct) + 1;
        var number = $"PAT-{DateTime.UtcNow.Year}-{sequence:000000}";
        while (await repository.PatientNumberExistsAsync(number, ct))
            number = $"PAT-{DateTime.UtcNow.Year}-{++sequence:000000}";
        var patient = new Patient(
            tenant.TenantId,
            number,
            command.FirstName,
            command.MiddleName,
            command.LastName,
            command.DateOfBirth,
            command.Gender,
            command.Phone,
            command.Email,
            command.NationalId,
            command.Address,
            command.EmergencyContactName,
            command.EmergencyContactPhone,
            command.MaritalStatus,
            command.Notes
        );
        await repository.AddAsync(patient, ct);
        await repository.SaveChangesAsync(ct);
        return Result<PatientDto>.Success(PatientDto.From(patient));
    }
}
