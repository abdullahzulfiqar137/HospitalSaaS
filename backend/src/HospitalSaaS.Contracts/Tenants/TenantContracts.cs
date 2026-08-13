namespace HospitalSaaS.Contracts.Tenants;

public sealed record CreateTenantRequest(string Name, string Slug);

public sealed record TenantResponse(Guid Id, string Name, string Slug, bool IsActive);
