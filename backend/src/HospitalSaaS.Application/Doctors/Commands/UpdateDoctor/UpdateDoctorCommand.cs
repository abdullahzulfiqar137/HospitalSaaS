using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Doctors.Models;

namespace HospitalSaaS.Application.Doctors.Commands.UpdateDoctor;

public sealed record UpdateDoctorCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Specialization,
    Guid DepartmentId,
    string LicenseNumber,
    string? Phone,
    string? Email,
    decimal ConsultationFee
) : ICommand<Result<DoctorDto>>;
