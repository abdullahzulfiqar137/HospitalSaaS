using Microsoft.AspNetCore.Authorization;

namespace HospitalSaaS.Api.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public const string Prefix = "Permission:";

    public HasPermissionAttribute(string permission)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);
        Policy = Prefix + permission;
    }
}
