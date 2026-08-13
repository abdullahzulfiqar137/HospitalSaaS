using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Prescriptions;

namespace HospitalSaaS.Application.Clinical;

public sealed record PrescriptionItemInput(
    Guid MedicineId,
    string Dose,
    string Frequency,
    string Duration,
    string Route,
    string? Instructions
);

public sealed record CreatePrescriptionCommand(
    Guid PatientId,
    Guid DoctorId,
    Guid? AppointmentId,
    string? Notes,
    IReadOnlyList<PrescriptionItemInput> Items
) : ICommand<Result<PrescriptionDto>>;

public sealed class CreatePrescriptionCommandValidator
    : AbstractValidator<CreatePrescriptionCommand>
{
    public CreatePrescriptionCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items)
            .ChildRules(i =>
            {
                i.RuleFor(x => x.MedicineId).NotEmpty();
                i.RuleFor(x => x.Dose).NotEmpty().MaximumLength(100);
                i.RuleFor(x => x.Frequency).NotEmpty().MaximumLength(100);
                i.RuleFor(x => x.Duration).NotEmpty().MaximumLength(100);
                i.RuleFor(x => x.Route).NotEmpty().MaximumLength(100);
            });
    }
}

public sealed class CreatePrescriptionCommandHandler(
    IClinicalRepository repo,
    ITenantContext tenant
) : IRequestHandler<CreatePrescriptionCommand, Result<PrescriptionDto>>
{
    public async Task<Result<PrescriptionDto>> Handle(
        CreatePrescriptionCommand x,
        CancellationToken ct
    )
    {
        var e = new Prescription(
            tenant.TenantId,
            x.PatientId,
            x.DoctorId,
            x.AppointmentId,
            x.Notes
        );
        foreach (var i in x.Items)
            e.AddItem(i.MedicineId, i.Dose, i.Frequency, i.Duration, i.Route, i.Instructions);
        await repo.AddPrescriptionAsync(e, ct);
        await repo.SaveChangesAsync(ct);
        return Result<PrescriptionDto>.Success(PrescriptionDto.From(e));
    }
}

public sealed record GetPrescriptionQuery(Guid Id) : IQuery<Result<PrescriptionDto>>;

public sealed class GetPrescriptionQueryHandler(IClinicalRepository repo)
    : IRequestHandler<GetPrescriptionQuery, Result<PrescriptionDto>>
{
    public async Task<Result<PrescriptionDto>> Handle(GetPrescriptionQuery x, CancellationToken ct)
    {
        var e = await repo.GetPrescriptionAsync(x.Id, ct);
        return e is null
            ? Result<PrescriptionDto>.Failure(
                new("Prescriptions.NotFound", "Prescription was not found.", ErrorType.NotFound)
            )
            : Result<PrescriptionDto>.Success(PrescriptionDto.From(e));
    }
}

public sealed record SearchPrescriptionsQuery(Guid? PatientId, int PageNumber, int PageSize)
    : IQuery<PagedResult<PrescriptionDto>>;

public sealed class SearchPrescriptionsQueryValidator : AbstractValidator<SearchPrescriptionsQuery>
{
    public SearchPrescriptionsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchPrescriptionsQueryHandler(IClinicalRepository repo)
    : IRequestHandler<SearchPrescriptionsQuery, PagedResult<PrescriptionDto>>
{
    public async Task<PagedResult<PrescriptionDto>> Handle(
        SearchPrescriptionsQuery x,
        CancellationToken ct
    )
    {
        var p = await repo.SearchPrescriptionsAsync(x.PatientId, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(PrescriptionDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}
