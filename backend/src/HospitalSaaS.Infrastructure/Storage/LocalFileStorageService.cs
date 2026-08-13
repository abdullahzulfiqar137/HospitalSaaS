using HospitalSaaS.Application.Common.Interfaces;

namespace HospitalSaaS.Infrastructure.Storage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _root = Path.Combine(AppContext.BaseDirectory, "storage");

    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken ct
    )
    {
        Directory.CreateDirectory(_root);
        var key = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
        await using var output = File.Create(Path.Combine(_root, key));
        await content.CopyToAsync(output, ct);
        return key;
    }

    public Task<Stream> OpenReadAsync(string key, CancellationToken ct) =>
        Task.FromResult<Stream>(File.OpenRead(SafePath(key)));

    public Task DeleteAsync(string key, CancellationToken ct)
    {
        File.Delete(SafePath(key));
        return Task.CompletedTask;
    }

    private string SafePath(string key)
    {
        var path = Path.GetFullPath(Path.Combine(_root, key));
        if (!path.StartsWith(Path.GetFullPath(_root), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid storage key.");
        return path;
    }
}
