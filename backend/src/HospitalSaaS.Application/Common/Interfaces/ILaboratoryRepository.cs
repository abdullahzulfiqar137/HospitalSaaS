using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Laboratory;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface ILaboratoryRepository
{
    Task AddTestAsync(LabTest test, CancellationToken ct);
    Task<LabTest?> GetTestAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<LabTest>> GetTestsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);
    Task<bool> TestCodeExistsAsync(string code, CancellationToken ct);
    Task<PagedResult<LabTest>> SearchTestsAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    );
    Task AddAsync(LabOrder order, CancellationToken ct);
    Task AddResultAsync(LabResult result, CancellationToken ct);
    Task<int> CountResultsAsync(Guid itemId, CancellationToken ct);
    Task<IReadOnlyList<LabResult>> GetResultsAsync(Guid orderId, CancellationToken ct);
    Task<LabOrder?> GetAsync(Guid id, bool tracking, CancellationToken ct);
    Task<PagedResult<LabOrder>> SearchAsync(
        Guid? patientId,
        LabOrderStatus? status,
        int page,
        int size,
        CancellationToken ct
    );
    Task SaveChangesAsync(CancellationToken ct);
}
