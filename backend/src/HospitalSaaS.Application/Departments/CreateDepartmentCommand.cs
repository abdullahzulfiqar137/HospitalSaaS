using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Departments;

public sealed record CreateDepartmentCommand(string Name, string Code, string? Description)
    : ICommand<Result<DepartmentDto>>;
