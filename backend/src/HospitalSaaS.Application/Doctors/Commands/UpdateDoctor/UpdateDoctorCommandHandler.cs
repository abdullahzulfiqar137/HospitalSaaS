using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Doctors.Models;

namespace HospitalSaaS.Application.Doctors.Commands.UpdateDoctor;

public sealed class UpdateDoctorCommandHandler(IDoctorRepository repository)
    : IRequestHandler<UpdateDoctorCommand, Result<DoctorDto>>
{
    public async Task<Result<DoctorDto>> Handle(
        UpdateDoctorCommand command,
        CancellationToken cancellationToken
    )
    {
        var doctor = await repository.GetAsync(command.Id, true, cancellationToken);
        if (doctor is null)
            return Result<DoctorDto>.Failure(
                new("Doctors.NotFound", "Doctor was not found.", ErrorType.NotFound)
            );
        if (
            await repository.LicenseNumberExistsAsync(
                command.LicenseNumber,
                command.Id,
                cancellationToken
            )
        )
            return Result<DoctorDto>.Failure(
                new("Doctors.LicenseConflict", "License number already exists.", ErrorType.Conflict)
            );

        doctor.Update(
            command.FirstName,
            command.LastName,
            command.Specialization,
            command.DepartmentId,
            command.LicenseNumber,
            command.Phone,
            command.Email,
            command.ConsultationFee
        );
        await repository.SaveChangesAsync(cancellationToken);
        return Result<DoctorDto>.Success(DoctorDto.From(doctor));
    }
}
