using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Patients.Models;

namespace HospitalSaaS.Application.Patients.Queries.GetPatientById;

public sealed record GetPatientByIdQuery(Guid Id) : IQuery<Result<PatientDto>>;
