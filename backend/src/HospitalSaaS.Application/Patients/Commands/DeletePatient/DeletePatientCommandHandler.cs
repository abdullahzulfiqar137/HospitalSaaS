using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Patients.Commands.DeletePatient;

public sealed class DeletePatientCommandHandler(IPatientRepository repository, ICurrentUser user)
    : IRequestHandler<DeletePatientCommand, Result>
{
    public async Task<Result> Handle(DeletePatientCommand command, CancellationToken ct)
    {
        var p = await repository.GetAsync(command.Id, ct);
        if (p is null)
            return Result.Failure(
                new("Patients.NotFound", "Patient was not found.", ErrorType.NotFound)
            );
        p.SoftDelete(DateTime.UtcNow, user.UserId);
        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
