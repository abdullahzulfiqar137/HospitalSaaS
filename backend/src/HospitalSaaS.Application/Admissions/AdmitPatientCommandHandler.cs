using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Admissions;

namespace HospitalSaaS.Application.Admissions;

public sealed class AdmitPatientCommandHandler(
    IAdmissionRepository repository,
    ITenantContext tenant
) : IRequestHandler<AdmitPatientCommand, Result<AdmissionDto>>
{
    public async Task<Result<AdmissionDto>> Handle(
        AdmitPatientCommand command,
        CancellationToken ct
    )
    {
        var room = await repository.GetRoomAsync(command.RoomId, ct);
        var bed = await repository.GetBedAsync(command.BedId, true, ct);
        if (
            room is null
            || room.WardId != command.WardId
            || bed is null
            || bed.RoomId != command.RoomId
        )
            return Result<AdmissionDto>.Failure(
                new(
                    "Beds.InvalidLocation",
                    "Ward, room, and bed do not form a valid location.",
                    ErrorType.Validation
                )
            );
        if (bed.IsOccupied)
            return Result<AdmissionDto>.Failure(
                new("Beds.Occupied", "Bed is occupied.", ErrorType.Conflict)
            );
        var year = DateTime.UtcNow.Year;
        var sequence = await repository.CountAdmissionsInYearAsync(year, ct) + 1;
        var number = $"ADM-{year}-{sequence:000000}";
        while (await repository.AdmissionNumberExistsAsync(number, ct))
            number = $"ADM-{year}-{++sequence:000000}";
        bed.Occupy();
        var admission = new Admission(
            tenant.TenantId,
            number,
            command.PatientId,
            command.DoctorId,
            command.AdmissionType.Trim(),
            command.Reason.Trim(),
            command.WardId,
            command.RoomId,
            command.BedId
        );
        await repository.AddAdmissionAsync(admission, ct);
        await repository.SaveChangesAsync(ct);
        return Result<AdmissionDto>.Success(AdmissionDto.From(admission));
    }
}
