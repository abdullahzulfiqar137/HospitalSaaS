namespace HospitalSaaS.Contracts.Roles;

public sealed record CreateRoleRequest(string Name, IReadOnlyCollection<string> Permissions);

public sealed record RoleResponse(Guid Id, string Name, IReadOnlyCollection<string> Permissions);
