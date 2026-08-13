using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Documents;

public sealed class Document : TenantEntity
{
    private Document() { }

    public Document(
        Guid tenantId,
        Guid patientId,
        string name,
        string contentType,
        long size,
        string storageKey
    )
        : base(tenantId)
    {
        PatientId = patientId;
        Name = name;
        ContentType = contentType;
        Size = size;
        StorageKey = storageKey;
    }

    public Guid PatientId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long Size { get; private set; }
    public string StorageKey { get; private set; } = string.Empty;
}
