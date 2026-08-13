using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Users;

namespace HospitalSaaS.Application.Administration;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    IReadOnlyCollection<Guid> RoleIds
) : ICommand<Result<UserDto>>;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(12).MaximumLength(128);
        RuleFor(x => x.RoleIds).NotNull();
    }
}

public sealed class CreateUserCommandHandler(
    IAdministrationRepository r,
    IUserPasswordHasher hasher,
    ITenantContext t
) : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand x, CancellationToken ct)
    {
        var email = x.Email.Trim().ToLowerInvariant();
        if (await r.UserEmailExistsAsync(email, ct))
            return Result<UserDto>.Failure(
                new("Users.EmailExists", "Email is already registered.", ErrorType.Conflict)
            );
        var ids = x.RoleIds.Distinct().ToList();
        var roles = await r.GetRolesAsync(ids, ct);
        if (roles.Count != ids.Count)
            return Result<UserDto>.Failure(
                new("Users.InvalidRoles", "One or more roles do not exist.", ErrorType.Validation)
            );
        var e = new User(t.TenantId, email, string.Empty);
        e.ChangePassword(hasher.Hash(e, x.Password));
        foreach (var role in roles)
            e.AssignRole(role);
        await r.AddUserAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<UserDto>.Success(UserDto.From(e));
    }
}

public sealed record SearchUsersQuery(string? Search, int PageNumber, int PageSize)
    : IQuery<PagedResult<UserDto>>;

public sealed class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchUsersQueryHandler(IAdministrationRepository r)
    : IRequestHandler<SearchUsersQuery, PagedResult<UserDto>>
{
    public async Task<PagedResult<UserDto>> Handle(SearchUsersQuery x, CancellationToken ct)
    {
        var p = await r.SearchUsersAsync(x.Search, x.PageNumber, x.PageSize, ct);
        return new(p.Items.Select(UserDto.From).ToList(), p.PageNumber, p.PageSize, p.TotalCount);
    }
}
