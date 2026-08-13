using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Roles;

namespace HospitalSaaS.Domain.Users;

public sealed class User : TenantEntity
{
    private readonly List<Role> _roles = [];

    private User() { }

    public User(Guid tenantId, string email, string passwordHash)
        : base(tenantId)
    {
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        IsActive = true;
    }

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles;

    public void ChangePassword(string passwordHash) => PasswordHash = passwordHash;

    public void AssignRole(Role role)
    {
        if (_roles.All(x => x.Id != role.Id))
            _roles.Add(role);
    }
}
