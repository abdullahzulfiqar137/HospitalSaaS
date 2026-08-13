using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Doctors.Models;
using HospitalSaaS.Domain.Doctors;

namespace HospitalSaaS.Application.Doctors.Commands.CreateDoctor;

public sealed class CreateDoctorCommandHandler(IDoctorRepository repository, ITenantContext tenant)
    : IRequestHandler<CreateDoctorCommand, Result<DoctorDto>>
{
    public async Task<Result<DoctorDto>> Handle(
        CreateDoctorCommand command,
        CancellationToken cancellationToken
    )
    {
        if (
            await repository.EmployeeNumberExistsAsync(
                command.EmployeeNumber,
                null,
                cancellationToken
            )
        )
            return Result<DoctorDto>.Failure(
                new(
                    "Doctors.EmployeeNumberConflict",
                    "Employee number already exists.",
                    ErrorType.Conflict
                )
            );
        if (
            await repository.LicenseNumberExistsAsync(
                command.LicenseNumber,
                null,
                cancellationToken
            )
        )
            return Result<DoctorDto>.Failure(
                new("Doctors.LicenseConflict", "License number already exists.", ErrorType.Conflict)
            );

        var doctor = new Doctor(
            tenant.TenantId,
            command.EmployeeNumber,
            command.FirstName,
            command.LastName,
            command.Specialization,
            command.DepartmentId,
            command.LicenseNumber,
            command.Phone,
            command.Email,
            command.ConsultationFee
        );
        await repository.AddAsync(doctor, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result<DoctorDto>.Success(DoctorDto.From(doctor));
    }
}
