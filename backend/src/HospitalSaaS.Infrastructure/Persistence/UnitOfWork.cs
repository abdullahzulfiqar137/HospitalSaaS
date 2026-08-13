using HospitalSaaS.Application.Common.Interfaces;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class UnitOfWork(HospitalDbContext db) : IUnitOfWork
{
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken ct
    )
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var result = await operation(ct);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return result;
    }
}
