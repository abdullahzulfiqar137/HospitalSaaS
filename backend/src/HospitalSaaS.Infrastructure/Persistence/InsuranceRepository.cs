using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Insurance;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class InsuranceRepository(HospitalDbContext db) : IInsuranceRepository
{
    public Task AddAsync(InsurancePolicy x, CancellationToken ct) =>
        db.InsurancePolicies.AddAsync(x, ct).AsTask();

    public Task<bool> PolicyNumberExistsAsync(string n, CancellationToken ct) =>
        db.InsurancePolicies.AnyAsync(x => x.PolicyNumber == n.Trim(), ct);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<InsurancePolicy>> SearchAsync(
        Guid? patientId,
        int page,
        int size,
        CancellationToken ct
    )
    {
        var q = db.InsurancePolicies.AsNoTracking();
        if (patientId.HasValue)
            q = q.Where(x => x.PatientId == patientId);
        var c = await q.CountAsync(ct);
        return new(
            await q.OrderByDescending(x => x.ValidTo)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct),
            page,
            size,
            c
        );
    }
}
