using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Insurance;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IInsuranceRepository
{
    Task AddAsync(InsurancePolicy x, CancellationToken ct);
    Task<bool> PolicyNumberExistsAsync(string number, CancellationToken ct);
    Task<PagedResult<InsurancePolicy>> SearchAsync(
        Guid? patientId,
        int page,
        int size,
        CancellationToken ct
    );
    Task SaveChangesAsync(CancellationToken ct);
}
