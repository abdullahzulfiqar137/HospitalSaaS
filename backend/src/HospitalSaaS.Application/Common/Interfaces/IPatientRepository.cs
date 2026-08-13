using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Patients;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IPatientRepository
{
    Task AddAsync(Patient patient, CancellationToken cancellationToken);
    Task<Patient?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> PatientNumberExistsAsync(string number, CancellationToken cancellationToken);
    Task<int> CountRegisteredInYearAsync(int year, CancellationToken cancellationToken);
    Task<PagedResult<Patient>> SearchAsync(
        string? search,
        int page,
        int pageSize,
        string sortBy,
        bool descending,
        CancellationToken cancellationToken
    );
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
