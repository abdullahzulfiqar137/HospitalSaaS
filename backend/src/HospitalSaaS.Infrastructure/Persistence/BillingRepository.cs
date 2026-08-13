using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Billing;
using HospitalSaaS.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class BillingRepository(HospitalDbContext db) : IBillingRepository
{
    public Task AddInvoiceAsync(Invoice x, CancellationToken ct) =>
        db.Invoices.AddAsync(x, ct).AsTask();

    public Task<Invoice?> GetInvoiceAsync(Guid id, bool tracking, CancellationToken ct) =>
        (tracking ? db.Invoices : db.Invoices.AsNoTracking())
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

    public Task AddPaymentAsync(Payment x, CancellationToken ct) =>
        db.Payments.AddAsync(x, ct).AsTask();

    public Task<Payment?> GetPaymentAsync(Guid id, bool tracking, CancellationToken ct) =>
        (tracking ? db.Payments : db.Payments.AsNoTracking()).SingleOrDefaultAsync(
            x => x.Id == id,
            ct
        );

    public async Task<decimal> GetNetPaidAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var sum = await db.Payments
                .Where(x => x.InvoiceId == id)
                .SumAsync(x => (decimal?)x.Amount - x.RefundedAmount, ct);
            return sum ?? 0m;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return 0m;
        }
    }

    public Task<int> CountInvoicesAsync(int y, CancellationToken ct) =>
        db.Invoices.CountAsync(x => x.IssuedAtUtc.Year == y, ct);

    public Task<int> CountPaymentsAsync(int y, CancellationToken ct) =>
        db.Payments.CountAsync(x => x.PaidAtUtc.Year == y, ct);

    public Task<bool> InvoiceNumberExistsAsync(string n, CancellationToken ct) =>
        db.Invoices.AnyAsync(x => x.InvoiceNumber == n, ct);

    public Task<bool> PaymentNumberExistsAsync(string n, CancellationToken ct) =>
        db.Payments.AnyAsync(x => x.PaymentNumber == n, ct);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<Invoice>> SearchInvoicesAsync(
        Guid? p,
        InvoiceStatus? s,
        int page,
        int size,
        CancellationToken ct
    )
    {
        try
        {
            IQueryable<Invoice> q = db.Invoices.AsNoTracking().Include(x => x.Items);
            if (p.HasValue)
                q = q.Where(x => x.PatientId == p);
            if (s.HasValue)
                q = q.Where(x => x.Status == s);
            var c = await q.CountAsync(ct);
            var items = await q.OrderByDescending(x => x.IssuedAtUtc)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct);
            return new(items, page, size, c);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new([], page, size, 0);
        }
    }

    public async Task<PagedResult<Payment>> SearchPaymentsAsync(
        Guid? i,
        int page,
        int size,
        CancellationToken ct
    )
    {
        try
        {
            var q = db.Payments.AsNoTracking();
            if (i.HasValue)
                q = q.Where(x => x.InvoiceId == i);
            var c = await q.CountAsync(ct);
            var items = await q.OrderByDescending(x => x.PaidAtUtc)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct);
            return new(items, page, size, c);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new([], page, size, 0);
        }
    }
}
