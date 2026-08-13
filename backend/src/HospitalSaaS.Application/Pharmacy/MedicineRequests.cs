using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Pharmacy;

namespace HospitalSaaS.Application.Pharmacy;

public sealed record CreateMedicineCommand(
    string Name,
    string GenericName,
    string? BrandName,
    string Strength,
    string DosageForm,
    string? Manufacturer,
    string? Barcode,
    decimal SalePrice,
    decimal PurchasePrice,
    int MinimumStockLevel
) : ICommand<Result<MedicineDto>>;

public sealed class CreateMedicineCommandValidator : AbstractValidator<CreateMedicineCommand>
{
    public CreateMedicineCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.GenericName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Strength).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DosageForm).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinimumStockLevel).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateMedicineCommandHandler(IPharmacyRepository repo, ITenantContext tenant)
    : IRequestHandler<CreateMedicineCommand, Result<MedicineDto>>
{
    public async Task<Result<MedicineDto>> Handle(CreateMedicineCommand x, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(x.Barcode) && await repo.BarcodeExistsAsync(x.Barcode, ct))
            return Result<MedicineDto>.Failure(
                new("Medicines.BarcodeConflict", "Barcode already exists.", ErrorType.Conflict)
            );
        var e = new Medicine(
            tenant.TenantId,
            x.Name.Trim(),
            x.GenericName.Trim(),
            x.BrandName,
            x.Strength.Trim(),
            x.DosageForm.Trim(),
            x.Manufacturer,
            x.Barcode,
            x.SalePrice,
            x.PurchasePrice,
            x.MinimumStockLevel
        );
        await repo.AddMedicineAsync(e, ct);
        await repo.SaveChangesAsync(ct);
        return Result<MedicineDto>.Success(MedicineDto.From(e));
    }
}

public sealed record GetMedicineQuery(Guid Id) : IQuery<Result<MedicineDto>>;

public sealed class GetMedicineQueryHandler(IPharmacyRepository repo)
    : IRequestHandler<GetMedicineQuery, Result<MedicineDto>>
{
    public async Task<Result<MedicineDto>> Handle(GetMedicineQuery x, CancellationToken ct)
    {
        var e = await repo.GetMedicineAsync(x.Id, false, ct);
        return e is null
            ? Result<MedicineDto>.Failure(
                new("Medicines.NotFound", "Medicine was not found.", ErrorType.NotFound)
            )
            : Result<MedicineDto>.Success(MedicineDto.From(e));
    }
}

public sealed record SearchMedicinesQuery(string? Search, int PageNumber, int PageSize)
    : IQuery<PagedResult<MedicineDto>>;

public sealed class SearchMedicinesQueryValidator : AbstractValidator<SearchMedicinesQuery>
{
    public SearchMedicinesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchMedicinesQueryHandler(IPharmacyRepository repo)
    : IRequestHandler<SearchMedicinesQuery, PagedResult<MedicineDto>>
{
    public async Task<PagedResult<MedicineDto>> Handle(SearchMedicinesQuery x, CancellationToken ct)
    {
        var p = await repo.SearchMedicinesAsync(x.Search, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(MedicineDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}

public sealed record DeleteMedicineCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteMedicineCommandHandler(IPharmacyRepository repo)
    : IRequestHandler<DeleteMedicineCommand, Result>
{
    public async Task<Result> Handle(DeleteMedicineCommand x, CancellationToken ct)
    {
        var e = await repo.GetMedicineAsync(x.Id, true, ct);
        if (e is null)
            return Result.Failure(
                new("Medicines.NotFound", "Medicine was not found.", ErrorType.NotFound)
            );
        repo.RemoveMedicine(e);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
