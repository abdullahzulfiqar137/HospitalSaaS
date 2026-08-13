using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Pharmacy;

public sealed class StockTransaction : TenantEntity
{
    private StockTransaction() { }

    public StockTransaction(
        Guid tenantId,
        Guid medicineId,
        StockTransactionType type,
        int quantity,
        string? reference
    )
        : base(tenantId)
    {
        if (quantity == 0)
            throw new DomainException("Stock quantity cannot be zero.");
        MedicineId = medicineId;
        Type = type;
        Quantity = quantity;
        Reference = reference;
        OccurredAtUtc = DateTime.UtcNow;
    }

    public Guid MedicineId { get; private set; }
    public StockTransactionType Type { get; private set; }
    public int Quantity { get; private set; }
    public string? Reference { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
}

public enum StockTransactionType
{
    Purchase,
    Dispense,
    Adjustment,
    Return,
    Transfer,
    Damage,
    Expired,
}
