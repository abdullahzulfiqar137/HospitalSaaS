using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Doctors.Commands.SetDoctorStatus;

public sealed record SetDoctorStatusCommand(Guid Id, bool IsActive) : ICommand<Result>;
