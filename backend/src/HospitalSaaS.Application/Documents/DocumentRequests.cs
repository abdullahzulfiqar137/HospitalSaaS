using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Documents;

namespace HospitalSaaS.Application.Documents;

public sealed record DocumentDto(
    Guid Id,
    Guid PatientId,
    string Name,
    string ContentType,
    long Size
)
{
    public static DocumentDto From(Document x) =>
        new(x.Id, x.PatientId, x.Name, x.ContentType, x.Size);
}

public sealed record CreateDocumentCommand(
    Guid PatientId,
    string Name,
    string ContentType,
    long Size,
    string StorageKey
) : ICommand<Result<DocumentDto>>;

public sealed class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
{
    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Size).InclusiveBetween(1, 20_000_000);
        RuleFor(x => x.StorageKey).NotEmpty();
    }
}

public sealed class CreateDocumentCommandHandler(IDocumentRepository r, ITenantContext t)
    : IRequestHandler<CreateDocumentCommand, Result<DocumentDto>>
{
    public async Task<Result<DocumentDto>> Handle(CreateDocumentCommand x, CancellationToken ct)
    {
        var e = new Document(t.TenantId, x.PatientId, x.Name, x.ContentType, x.Size, x.StorageKey);
        await r.AddAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<DocumentDto>.Success(DocumentDto.From(e));
    }
}

public sealed record SearchDocumentsQuery(Guid? PatientId, int PageNumber, int PageSize)
    : IQuery<PagedResult<DocumentDto>>;

public sealed class SearchDocumentsQueryValidator : AbstractValidator<SearchDocumentsQuery>
{
    public SearchDocumentsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchDocumentsQueryHandler(IDocumentRepository r)
    : IRequestHandler<SearchDocumentsQuery, PagedResult<DocumentDto>>
{
    public async Task<PagedResult<DocumentDto>> Handle(SearchDocumentsQuery x, CancellationToken ct)
    {
        var p = await r.SearchAsync(x.PatientId, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(DocumentDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}

public sealed record DeleteDocumentCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteDocumentCommandHandler(IDocumentRepository r, IFileStorageService storage)
    : IRequestHandler<DeleteDocumentCommand, Result>
{
    public async Task<Result> Handle(DeleteDocumentCommand x, CancellationToken ct)
    {
        var e = await r.GetAsync(x.Id, true, ct);
        if (e is null)
            return Result.Failure(
                new("Documents.NotFound", "Document was not found.", ErrorType.NotFound)
            );
        await storage.DeleteAsync(e.StorageKey, ct);
        r.Remove(e);
        await r.SaveChangesAsync(ct);
        return Result.Success();
    }
}
