using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Tenants;

namespace HospitalSaaS.Application.Administration;

public sealed record CreateTenantCommand(string Name, string Slug) : ICommand<Result<TenantDto>>;

public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100).Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$");
    }
}

public sealed class CreateTenantCommandHandler(IAdministrationRepository r)
    : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
{
    public async Task<Result<TenantDto>> Handle(CreateTenantCommand x, CancellationToken ct)
    {
        if (await r.TenantSlugExistsAsync(x.Slug, ct))
            return Result<TenantDto>.Failure(
                new("Tenants.SlugConflict", "Tenant slug already exists.", ErrorType.Conflict)
            );
        var e = new Tenant(x.Name.Trim(), x.Slug.Trim().ToLowerInvariant());
        await r.AddTenantAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<TenantDto>.Success(TenantDto.From(e));
    }
}
