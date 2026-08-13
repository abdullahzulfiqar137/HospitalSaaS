using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HospitalSaaS.Api.Authorization;

public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(HasPermissionAttribute.Prefix, StringComparison.Ordinal))
            return base.GetPolicyAsync(policyName);

        var permission = policyName[HasPermissionAttribute.Prefix.Length..];
        if (string.IsNullOrWhiteSpace(permission))
            return Task.FromResult<AuthorizationPolicy?>(null);

        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permission))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
