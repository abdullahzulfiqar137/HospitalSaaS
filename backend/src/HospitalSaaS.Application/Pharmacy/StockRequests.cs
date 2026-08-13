using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Pharmacy;

namespace HospitalSaaS.Application.Pharmacy;

public sealed record RecordMedicineStockCommand(
    Guid MedicineId,
    StockTransactionType Type,
    int Quantity,
    string? Reference
) : ICommand<Result<StockTransactionDto>>;

public sealed class RecordMedicineStockCommandValidator
    : AbstractValidator<RecordMedicineStockCommand>
{
    public RecordMedicineStockCommandValidator()
    {
        RuleFor(x => x.MedicineId).NotEmpty();
        RuleFor(x => x.Quantity).NotEqual(0);
        RuleFor(x => x.Reference).MaximumLength(200);
    }
}

public sealed class RecordMedicineStockCommandHandler(
    IPharmacyRepository repo,
    ITenantContext tenant
) : IRequestHandler<RecordMedicineStockCommand, Result<StockTransactionDto>>
{
    public async Task<Result<StockTransactionDto>> Handle(
        RecordMedicineStockCommand x,
        CancellationToken ct
    )
    {
        if (await repo.GetMedicineAsync(x.MedicineId, false, ct) is null)
            return Result<StockTransactionDto>.Failure(
                new("Medicines.NotFound", "Medicine was not found.", ErrorType.NotFound)
            );
        var signed = x.Type == StockTransactionType.Dispense ? -Math.Abs(x.Quantity) : x.Quantity;
        if (await repo.GetStockBalanceAsync(x.MedicineId, ct) + signed < 0)
            return Result<StockTransactionDto>.Failure(
                new("Stock.Insufficient", "Insufficient medicine stock.", ErrorType.Conflict)
            );
        var e = new StockTransaction(tenant.TenantId, x.MedicineId, x.Type, signed, x.Reference);
        await repo.AddStockTransactionAsync(e, ct);
        await repo.SaveChangesAsync(ct);
        return Result<StockTransactionDto>.Success(StockTransactionDto.From(e));
    }
}

public sealed record GetMedicineStockQuery(Guid MedicineId) : IQuery<Result<int>>;

public sealed class GetMedicineStockQueryHandler(IPharmacyRepository repo)
    : IRequestHandler<GetMedicineStockQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetMedicineStockQuery x, CancellationToken ct)
    {
        if (await repo.GetMedicineAsync(x.MedicineId, false, ct) is null)
            return Result<int>.Failure(
                new("Medicines.NotFound", "Medicine was not found.", ErrorType.NotFound)
            );
        return Result<int>.Success(await repo.GetStockBalanceAsync(x.MedicineId, ct));
    }
}
