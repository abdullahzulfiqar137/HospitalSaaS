using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Patients;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class PatientRepository(HospitalDbContext db) : IPatientRepository
{
    public Task AddAsync(Patient p, CancellationToken ct) => db.Patients.AddAsync(p, ct).AsTask();

    public Task<Patient?> GetAsync(Guid id, CancellationToken ct) =>
        db.Patients.SingleOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> PatientNumberExistsAsync(string n, CancellationToken ct) =>
        db.Patients.AnyAsync(x => x.PatientNumber == n, ct);

    public Task<int> CountRegisteredInYearAsync(int y, CancellationToken ct) =>
        db.Patients.CountAsync(x => x.RegistrationDateUtc.Year == y, ct);

    public async Task<PagedResult<Patient>> SearchAsync(
        string? s,
        int page,
        int size,
        string sortBy,
        bool desc,
        CancellationToken ct
    )
    {
        IQueryable<Patient> q = db.Patients.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(s))
        {
            s = s.Trim();
            q = q.Where(x =>
                x.PatientNumber.Contains(s)
                || x.FirstName.Contains(s)
                || x.LastName.Contains(s)
                || (x.Phone != null && x.Phone.Contains(s))
                || (x.Email != null && x.Email.Contains(s))
                || (x.NationalId != null && x.NationalId.Contains(s))
            );
        }
        q = (sortBy.ToLowerInvariant(), desc) switch
        {
            ("firstname", false) => q.OrderBy(x => x.FirstName),
            ("firstname", true) => q.OrderByDescending(x => x.FirstName),
            ("patientnumber", false) => q.OrderBy(x => x.PatientNumber),
            ("patientnumber", true) => q.OrderByDescending(x => x.PatientNumber),
            (_, false) => q.OrderBy(x => x.CreatedAtUtc),
            _ => q.OrderByDescending(x => x.CreatedAtUtc),
        };
        try
        {
            var count = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * size).Take(size).ToListAsync(ct);
            return new(items, page, size, count);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new([], page, size, 0);
        }
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
