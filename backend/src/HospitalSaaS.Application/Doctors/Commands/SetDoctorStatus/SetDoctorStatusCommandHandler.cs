using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Doctors.Commands.SetDoctorStatus;

public sealed class SetDoctorStatusCommandHandler(IDoctorRepository repository)
    : IRequestHandler<SetDoctorStatusCommand, Result>
{
    public async Task<Result> Handle(
        SetDoctorStatusCommand command,
        CancellationToken cancellationToken
    )
    {
        var doctor = await repository.GetAsync(command.Id, true, cancellationToken);
        if (doctor is null)
            return Result.Failure(
                new("Doctors.NotFound", "Doctor was not found.", ErrorType.NotFound)
            );
        if (command.IsActive)
            doctor.Activate();
        else
            doctor.Deactivate();
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
