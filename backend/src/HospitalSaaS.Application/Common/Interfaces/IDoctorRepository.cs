using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Doctors;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IDoctorRepository
{
    Task AddAsync(Doctor doctor, CancellationToken cancellationToken);
    Task<Doctor?> GetAsync(Guid id, bool tracking, CancellationToken cancellationToken);
    Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        Guid? excludingId,
        CancellationToken cancellationToken
    );
    Task<bool> LicenseNumberExistsAsync(
        string licenseNumber,
        Guid? excludingId,
        CancellationToken cancellationToken
    );
    Task<PagedResult<Doctor>> SearchAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    );
    void Remove(Doctor doctor);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
