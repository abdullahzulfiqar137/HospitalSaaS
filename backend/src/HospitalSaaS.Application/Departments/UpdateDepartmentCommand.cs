using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Departments;

public sealed record UpdateDepartmentCommand(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    Guid? HeadDoctorId
) : ICommand<Result<DepartmentDto>>;
