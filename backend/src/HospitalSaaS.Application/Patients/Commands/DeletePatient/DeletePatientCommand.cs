using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Patients.Commands.DeletePatient;

public sealed record DeletePatientCommand(Guid Id) : ICommand<Result>;
