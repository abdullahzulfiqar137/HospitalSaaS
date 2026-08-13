using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Radiology;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IRadiologyRepository
{
    Task AddTestAsync(RadiologyTest test, CancellationToken cancellationToken);
    Task<RadiologyTest?> GetTestAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> TestCodeExistsAsync(string code, CancellationToken cancellationToken);
    Task<PagedResult<RadiologyTest>> SearchTestsAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task AddAsync(RadiologyOrder order, CancellationToken cancellationToken);
    Task AddResultAsync(RadiologyResult result, CancellationToken cancellationToken);
    Task<int> CountResultsAsync(Guid orderId, CancellationToken cancellationToken);
    Task<IReadOnlyList<RadiologyResult>> GetResultsAsync(
        Guid orderId,
        CancellationToken cancellationToken
    );
    Task<RadiologyOrder?> GetAsync(Guid id, bool tracking, CancellationToken cancellationToken);
    Task<PagedResult<RadiologyOrder>> SearchAsync(
        Guid? patientId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
