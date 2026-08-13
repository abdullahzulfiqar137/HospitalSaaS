using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Admissions;

namespace HospitalSaaS.Application.Admissions;

public sealed class CreateWardCommandHandler(IAdmissionRepository repository, ITenantContext tenant)
    : IRequestHandler<CreateWardCommand, Result<WardDto>>
{
    public async Task<Result<WardDto>> Handle(CreateWardCommand command, CancellationToken ct)
    {
        if (await repository.WardCodeExistsAsync(command.Code, ct))
            return Result<WardDto>.Failure(
                new("Wards.CodeConflict", "Ward code already exists.", ErrorType.Conflict)
            );
        var ward = new Ward(
            tenant.TenantId,
            command.Name.Trim(),
            command.Code.Trim().ToUpperInvariant()
        );
        await repository.AddWardAsync(ward, ct);
        await repository.SaveChangesAsync(ct);
        return Result<WardDto>.Success(WardDto.From(ward));
    }
}

public sealed class CreateRoomCommandHandler(IAdmissionRepository repository, ITenantContext tenant)
    : IRequestHandler<CreateRoomCommand, Result<RoomDto>>
{
    public async Task<Result<RoomDto>> Handle(CreateRoomCommand command, CancellationToken ct)
    {
        if (await repository.GetWardAsync(command.WardId, ct) is null)
            return Result<RoomDto>.Failure(
                new("Wards.NotFound", "Ward was not found.", ErrorType.NotFound)
            );
        if (await repository.RoomNumberExistsAsync(command.WardId, command.Number, ct))
            return Result<RoomDto>.Failure(
                new(
                    "Rooms.NumberConflict",
                    "Room number already exists in this ward.",
                    ErrorType.Conflict
                )
            );
        var room = new Room(tenant.TenantId, command.WardId, command.Number.Trim());
        await repository.AddRoomAsync(room, ct);
        await repository.SaveChangesAsync(ct);
        return Result<RoomDto>.Success(RoomDto.From(room));
    }
}

public sealed class CreateBedCommandHandler(IAdmissionRepository repository, ITenantContext tenant)
    : IRequestHandler<CreateBedCommand, Result<BedDto>>
{
    public async Task<Result<BedDto>> Handle(CreateBedCommand command, CancellationToken ct)
    {
        if (await repository.GetRoomAsync(command.RoomId, ct) is null)
            return Result<BedDto>.Failure(
                new("Rooms.NotFound", "Room was not found.", ErrorType.NotFound)
            );
        if (await repository.BedNumberExistsAsync(command.RoomId, command.Number, ct))
            return Result<BedDto>.Failure(
                new(
                    "Beds.NumberConflict",
                    "Bed number already exists in this room.",
                    ErrorType.Conflict
                )
            );
        var bed = new Bed(tenant.TenantId, command.RoomId, command.Number.Trim());
        await repository.AddBedAsync(bed, ct);
        await repository.SaveChangesAsync(ct);
        return Result<BedDto>.Success(BedDto.From(bed));
    }
}
