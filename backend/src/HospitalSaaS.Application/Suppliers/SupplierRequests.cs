using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Suppliers;

namespace HospitalSaaS.Application.Suppliers;

public sealed record SupplierDto(
    Guid Id,
    string Name,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address,
    bool IsActive
)
{
    public static SupplierDto From(Supplier x) =>
        new(x.Id, x.Name, x.ContactName, x.Phone, x.Email, x.Address, x.IsActive);
}

public sealed record CreateSupplierCommand(
    string Name,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address
) : ICommand<Result<SupplierDto>>;

public sealed class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(30);
    }
}

public sealed class CreateSupplierCommandHandler(ISupplierRepository r, ITenantContext t)
    : IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(CreateSupplierCommand x, CancellationToken ct)
    {
        var e = new Supplier(
            t.TenantId,
            x.Name.Trim(),
            x.ContactName,
            x.Phone,
            x.Email?.Trim().ToLowerInvariant(),
            x.Address
        );
        await r.AddAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<SupplierDto>.Success(SupplierDto.From(e));
    }
}

public sealed record GetSupplierQuery(Guid Id) : IQuery<Result<SupplierDto>>;

public sealed class GetSupplierQueryHandler(ISupplierRepository r)
    : IRequestHandler<GetSupplierQuery, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(GetSupplierQuery x, CancellationToken ct)
    {
        var e = await r.GetAsync(x.Id, false, ct);
        return e is null
            ? Result<SupplierDto>.Failure(
                new("Suppliers.NotFound", "Supplier was not found.", ErrorType.NotFound)
            )
            : Result<SupplierDto>.Success(SupplierDto.From(e));
    }
}

public sealed record SearchSuppliersQuery(string? Search, int PageNumber, int PageSize)
    : IQuery<PagedResult<SupplierDto>>;

public sealed class SearchSuppliersQueryValidator : AbstractValidator<SearchSuppliersQuery>
{
    public SearchSuppliersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchSuppliersQueryHandler(ISupplierRepository r)
    : IRequestHandler<SearchSuppliersQuery, PagedResult<SupplierDto>>
{
    public async Task<PagedResult<SupplierDto>> Handle(SearchSuppliersQuery x, CancellationToken ct)
    {
        var p = await r.SearchAsync(x.Search, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(SupplierDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}

public sealed record DeleteSupplierCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteSupplierCommandHandler(ISupplierRepository r)
    : IRequestHandler<DeleteSupplierCommand, Result>
{
    public async Task<Result> Handle(DeleteSupplierCommand x, CancellationToken ct)
    {
        var e = await r.GetAsync(x.Id, true, ct);
        if (e is null)
            return Result.Failure(
                new("Suppliers.NotFound", "Supplier was not found.", ErrorType.NotFound)
            );
        r.Remove(e);
        await r.SaveChangesAsync(ct);
        return Result.Success();
    }
}
