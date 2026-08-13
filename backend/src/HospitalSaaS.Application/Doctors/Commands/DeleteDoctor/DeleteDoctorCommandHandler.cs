using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Doctors.Commands.DeleteDoctor;

public sealed class DeleteDoctorCommandHandler(IDoctorRepository repository)
    : IRequestHandler<DeleteDoctorCommand, Result>
{
    public async Task<Result> Handle(
        DeleteDoctorCommand command,
        CancellationToken cancellationToken
    )
    {
        var doctor = await repository.GetAsync(command.Id, true, cancellationToken);
        if (doctor is null)
            return Result.Failure(
                new("Doctors.NotFound", "Doctor was not found.", ErrorType.NotFound)
            );
        repository.Remove(doctor);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
