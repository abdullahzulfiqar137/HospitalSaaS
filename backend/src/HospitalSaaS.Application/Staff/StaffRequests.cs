using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Staff;

namespace HospitalSaaS.Application.Staff;

public sealed record StaffDto(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string JobTitle,
    Guid? DepartmentId,
    bool IsActive
)
{
    public static StaffDto From(StaffMember x) =>
        new(
            x.Id,
            x.EmployeeNumber,
            x.FirstName,
            x.LastName,
            x.JobTitle,
            x.DepartmentId,
            x.IsActive
        );
}

public sealed record CreateStaffCommand(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string JobTitle,
    Guid? DepartmentId
) : ICommand<Result<StaffDto>>;

public sealed class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
{
    public CreateStaffCommandValidator()
    {
        RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(150);
    }
}

public sealed class CreateStaffCommandHandler(IStaffRepository r, ITenantContext t)
    : IRequestHandler<CreateStaffCommand, Result<StaffDto>>
{
    public async Task<Result<StaffDto>> Handle(CreateStaffCommand x, CancellationToken ct)
    {
        if (await r.EmployeeNumberExistsAsync(x.EmployeeNumber, ct))
            return Result<StaffDto>.Failure(
                new(
                    "Staff.EmployeeNumberConflict",
                    "Employee number already exists.",
                    ErrorType.Conflict
                )
            );
        var e = new StaffMember(
            t.TenantId,
            x.EmployeeNumber.Trim().ToUpperInvariant(),
            x.FirstName.Trim(),
            x.LastName.Trim(),
            x.JobTitle.Trim(),
            x.DepartmentId
        );
        await r.AddAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<StaffDto>.Success(StaffDto.From(e));
    }
}

public sealed record SearchStaffQuery(string? Search, int PageNumber, int PageSize)
    : IQuery<PagedResult<StaffDto>>;

public sealed class SearchStaffQueryValidator : AbstractValidator<SearchStaffQuery>
{
    public SearchStaffQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchStaffQueryHandler(IStaffRepository r)
    : IRequestHandler<SearchStaffQuery, PagedResult<StaffDto>>
{
    public async Task<PagedResult<StaffDto>> Handle(SearchStaffQuery x, CancellationToken ct)
    {
        var p = await r.SearchAsync(x.Search, x.PageNumber, x.PageSize, ct);
        return new(p.Items.Select(StaffDto.From).ToList(), p.PageNumber, p.PageSize, p.TotalCount);
    }
}
