using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Doctors.Commands.DeleteDoctor;

public sealed record DeleteDoctorCommand(Guid Id) : ICommand<Result>;
