using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Admissions;

public sealed record DischargeAdmissionCommand(Guid AdmissionId) : ICommand<Result>;

public sealed class DischargeAdmissionCommandHandler(IAdmissionRepository repository)
    : IRequestHandler<DischargeAdmissionCommand, Result>
{
    public async Task<Result> Handle(DischargeAdmissionCommand command, CancellationToken ct)
    {
        var admission = await repository.GetAdmissionAsync(command.AdmissionId, true, ct);
        if (admission is null)
            return Result.Failure(
                new("Admissions.NotFound", "Admission was not found.", ErrorType.NotFound)
            );
        var bed = await repository.GetBedAsync(admission.BedId, true, ct);
        if (bed is null)
            return Result.Failure(
                new("Beds.NotFound", "Assigned bed was not found.", ErrorType.Unexpected)
            );
        admission.Discharge();
        bed.Release();
        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
