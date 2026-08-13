using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Admissions;

public sealed record TransferAdmissionCommand(
    Guid AdmissionId,
    Guid WardId,
    Guid RoomId,
    Guid BedId,
    string? Reason = null
) : ICommand<Result<AdmissionDto>>;

public sealed class TransferAdmissionCommandValidator : AbstractValidator<TransferAdmissionCommand>
{
    public TransferAdmissionCommandValidator()
    {
        RuleFor(x => x.AdmissionId).NotEmpty();
        RuleFor(x => x.WardId).NotEmpty();
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.BedId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}

public sealed class TransferAdmissionCommandHandler(IAdmissionRepository repository)
    : IRequestHandler<TransferAdmissionCommand, Result<AdmissionDto>>
{
    public async Task<Result<AdmissionDto>> Handle(
        TransferAdmissionCommand command,
        CancellationToken ct
    )
    {
        var admission = await repository.GetAdmissionAsync(command.AdmissionId, true, ct);
        if (admission is null)
            return Result<AdmissionDto>.Failure(
                new("Admissions.NotFound", "Admission was not found.", ErrorType.NotFound)
            );
        if (admission.BedId == command.BedId)
            return Result<AdmissionDto>.Failure(
                new(
                    "Admissions.SameBed",
                    "Patient is already assigned to this bed.",
                    ErrorType.Validation
                )
            );

        var room = await repository.GetRoomAsync(command.RoomId, ct);
        var targetBed = await repository.GetBedAsync(command.BedId, true, ct);
        if (
            room is null
            || room.WardId != command.WardId
            || targetBed is null
            || targetBed.RoomId != command.RoomId
        )
            return Result<AdmissionDto>.Failure(
                new(
                    "Beds.InvalidLocation",
                    "Ward, room, and bed do not form a valid location.",
                    ErrorType.Validation
                )
            );
        if (targetBed.IsOccupied)
            return Result<AdmissionDto>.Failure(
                new("Beds.Occupied", "Target bed is occupied.", ErrorType.Conflict)
            );

        var currentBed = await repository.GetBedAsync(admission.BedId, true, ct);
        if (currentBed is null)
            return Result<AdmissionDto>.Failure(
                new(
                    "Beds.CurrentNotFound",
                    "The currently assigned bed was not found.",
                    ErrorType.Unexpected
                )
            );
        var history = new HospitalSaaS.Domain.Admissions.AdmissionHistory(
            admission.TenantId,
            admission.Id,
            admission.WardId,
            admission.RoomId,
            admission.BedId,
            command.WardId,
            command.RoomId,
            command.BedId,
            command.Reason
        );
        currentBed.Release();
        targetBed.Occupy();
        admission.Transfer(command.WardId, command.RoomId, command.BedId);
        await repository.AddHistoryAsync(history, ct);
        await repository.SaveChangesAsync(ct);
        return Result<AdmissionDto>.Success(AdmissionDto.From(admission));
    }
}
