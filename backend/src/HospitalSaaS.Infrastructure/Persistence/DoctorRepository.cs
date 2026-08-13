using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Doctors;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class DoctorRepository(HospitalDbContext dbContext) : IDoctorRepository
{
    public Task AddAsync(Doctor doctor, CancellationToken cancellationToken) =>
        dbContext.Doctors.AddAsync(doctor, cancellationToken).AsTask();

    public Task<Doctor?> GetAsync(Guid id, bool tracking, CancellationToken cancellationToken)
    {
        var query = tracking ? dbContext.Doctors : dbContext.Doctors.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        Guid? excludingId,
        CancellationToken cancellationToken
    ) =>
        dbContext.Doctors.AnyAsync(
            x =>
                x.EmployeeNumber == employeeNumber.Trim()
                && (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken
        );

    public Task<bool> LicenseNumberExistsAsync(
        string licenseNumber,
        Guid? excludingId,
        CancellationToken cancellationToken
    ) =>
        dbContext.Doctors.AnyAsync(
            x =>
                x.LicenseNumber == licenseNumber.Trim()
                && (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken
        );

    public async Task<PagedResult<Doctor>> SearchAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = dbContext.Doctors.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.FirstName.Contains(term)
                || x.LastName.Contains(term)
                || x.EmployeeNumber.Contains(term)
                || x.Specialization.Contains(term)
                || x.LicenseNumber.Contains(term)
                || (x.Email != null && x.Email.Contains(term))
            );
        }

        try
        {
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return new PagedResult<Doctor>(items, pageNumber, pageSize, totalCount);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return new PagedResult<Doctor>([], pageNumber, pageSize, 0);
        }
    }

    public void Remove(Doctor doctor) => dbContext.Doctors.Remove(doctor);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
