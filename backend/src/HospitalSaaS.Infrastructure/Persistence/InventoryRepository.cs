using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Inventory;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class InventoryRepository(HospitalDbContext db) : IInventoryRepository
{
    public Task AddItemAsync(InventoryItem x, CancellationToken ct) =>
        db.InventoryItems.AddAsync(x, ct).AsTask();

    public Task<InventoryItem?> GetItemAsync(Guid id, bool tracking, CancellationToken ct) =>
        (tracking ? db.InventoryItems : db.InventoryItems.AsNoTracking()).SingleOrDefaultAsync(
            x => x.Id == id,
            ct
        );

    public Task<bool> SkuExistsAsync(string sku, CancellationToken ct) =>
        db.InventoryItems.AnyAsync(x => x.Sku == sku.Trim().ToUpper(), ct);

    public void RemoveItem(InventoryItem x) => db.InventoryItems.Remove(x);

    public Task AddTransactionAsync(InventoryTransaction x, CancellationToken ct) =>
        db.InventoryTransactions.AddAsync(x, ct).AsTask();

    public Task<int> GetBalanceAsync(Guid id, CancellationToken ct) =>
        db
            .InventoryTransactions.Where(x => x.InventoryItemId == id)
            .SumAsync(x => (int?)x.Quantity, ct)
            .ContinueWith(x => x.Result ?? 0, ct);

    public Task AddPurchaseOrderAsync(PurchaseOrder order, CancellationToken ct) =>
        db.PurchaseOrders.AddAsync(order, ct).AsTask();

    public Task<PurchaseOrder?> GetPurchaseOrderAsync(
        Guid id,
        bool tracking,
        CancellationToken ct
    ) =>
        (tracking ? db.PurchaseOrders : db.PurchaseOrders.AsNoTracking())
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedResult<PurchaseOrder>> SearchPurchaseOrdersAsync(
        PurchaseOrderStatus? status,
        int page,
        int size,
        CancellationToken ct
    )
    {
        IQueryable<PurchaseOrder> query = db.PurchaseOrders.AsNoTracking().Include(x => x.Items);
        if (status.HasValue)
            query = query.Where(x => x.Status == status);
        var count = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.OrderedAtUtc)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);
        return new(items, page, size, count);
    }

    public Task<int> CountPurchaseOrdersInYearAsync(int year, CancellationToken ct) =>
        db.PurchaseOrders.CountAsync(x => x.OrderedAtUtc.Year == year, ct);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<InventoryItem>> SearchAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    )
    {
        var q = db.InventoryItems.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(x => x.Sku.Contains(s) || x.Name.Contains(s));
        }
        var c = await q.CountAsync(ct);
        return new(
            await q.OrderBy(x => x.Name).Skip((page - 1) * size).Take(size).ToListAsync(ct),
            page,
            size,
            c
        );
    }
}
