using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Admissions;

public sealed record CreateWardCommand(string Name, string Code) : ICommand<Result<WardDto>>;

public sealed record CreateRoomCommand(Guid WardId, string Number) : ICommand<Result<RoomDto>>;

public sealed record CreateBedCommand(Guid RoomId, string Number) : ICommand<Result<BedDto>>;
