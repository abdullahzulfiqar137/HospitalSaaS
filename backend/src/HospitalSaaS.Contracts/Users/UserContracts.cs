namespace HospitalSaaS.Contracts.Users;

public sealed record CreateUserRequest(
    string Email,
    string Password,
    IReadOnlyCollection<Guid> RoleIds
);

public sealed record UserResponse(
    Guid Id,
    string Email,
    bool IsActive,
    IReadOnlyCollection<string> Roles
);
