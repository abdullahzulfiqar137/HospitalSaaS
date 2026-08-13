using HospitalSaaS.Domain.Departments;

namespace HospitalSaaS.Application.Departments;

public sealed record DepartmentDto(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    Guid? HeadDoctorId,
    DepartmentStatus Status
)
{
    public static DepartmentDto From(Department value) =>
        new(value.Id, value.Name, value.Code, value.Description, value.HeadDoctorId, value.Status);
}
