using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Inventory;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IInventoryRepository
{
    Task AddItemAsync(InventoryItem item, CancellationToken ct);
    Task<InventoryItem?> GetItemAsync(Guid id, bool tracking, CancellationToken ct);
    Task<bool> SkuExistsAsync(string sku, CancellationToken ct);
    Task<PagedResult<InventoryItem>> SearchAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    );
    void RemoveItem(InventoryItem item);
    Task AddTransactionAsync(InventoryTransaction transaction, CancellationToken ct);
    Task<int> GetBalanceAsync(Guid itemId, CancellationToken ct);
    Task AddPurchaseOrderAsync(PurchaseOrder order, CancellationToken ct);
    Task<PurchaseOrder?> GetPurchaseOrderAsync(Guid id, bool tracking, CancellationToken ct);
    Task<PagedResult<PurchaseOrder>> SearchPurchaseOrdersAsync(
        PurchaseOrderStatus? status,
        int page,
        int size,
        CancellationToken ct
    );
    Task<int> CountPurchaseOrdersInYearAsync(int year, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
