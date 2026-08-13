using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Doctors.Models;

namespace HospitalSaaS.Application.Doctors.Queries.GetDoctorById;

public sealed record GetDoctorByIdQuery(Guid Id) : IQuery<Result<DoctorDto>>;
