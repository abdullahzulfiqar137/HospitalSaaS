using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Roles;

namespace HospitalSaaS.Application.Administration;

public sealed record CreateRoleCommand(string Name, IReadOnlyCollection<string> Permissions)
    : ICommand<Result<RoleDto>>;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Permissions).NotNull();
        RuleForEach(x => x.Permissions).NotEmpty().MaximumLength(150);
    }
}

public sealed class CreateRoleCommandHandler(IAdministrationRepository r, ITenantContext t)
    : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(CreateRoleCommand x, CancellationToken ct)
    {
        if (await r.RoleNameExistsAsync(x.Name, ct))
            return Result<RoleDto>.Failure(
                new("Roles.NameConflict", "Role name already exists.", ErrorType.Conflict)
            );
        var requested = x.Permissions.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var granted = await r.GetPermissionsAsync(requested, ct);
        if (granted.Count != requested.Count)
            return Result<RoleDto>.Failure(
                new(
                    "Roles.InvalidPermissions",
                    "One or more permissions do not exist.",
                    ErrorType.Validation
                )
            );
        var e = new Role(t.TenantId, x.Name.Trim());
        foreach (var p in granted)
            e.Grant(p);
        await r.AddRoleAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<RoleDto>.Success(RoleDto.From(e));
    }
}

public sealed record SearchRolesQuery(int PageNumber, int PageSize) : IQuery<PagedResult<RoleDto>>;

public sealed class SearchRolesQueryValidator : AbstractValidator<SearchRolesQuery>
{
    public SearchRolesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchRolesQueryHandler(IAdministrationRepository r)
    : IRequestHandler<SearchRolesQuery, PagedResult<RoleDto>>
{
    public async Task<PagedResult<RoleDto>> Handle(SearchRolesQuery x, CancellationToken ct)
    {
        var p = await r.SearchRolesAsync(x.PageNumber, x.PageSize, ct);
        return new(p.Items.Select(RoleDto.From).ToList(), p.PageNumber, p.PageSize, p.TotalCount);
    }
}
