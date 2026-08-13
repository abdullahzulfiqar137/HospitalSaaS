using HospitalSaaS.Domain.Doctors;

namespace HospitalSaaS.Application.Doctors.Models;

public sealed record DoctorDto(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Specialization,
    Guid DepartmentId,
    string LicenseNumber,
    string? Phone,
    string? Email,
    decimal ConsultationFee,
    DoctorStatus Status
)
{
    public static DoctorDto From(Doctor doctor) =>
        new(
            doctor.Id,
            doctor.EmployeeNumber,
            doctor.FirstName,
            doctor.LastName,
            doctor.Specialization,
            doctor.DepartmentId,
            doctor.LicenseNumber,
            doctor.Phone,
            doctor.Email,
            doctor.ConsultationFee,
            doctor.Status
        );
}
