using System.Security.Claims;
using HospitalSaaS.Application.Common.Interfaces;

namespace HospitalSaaS.Api.Services;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;
    public Guid? UserId =>
        Guid.TryParse(
            Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Principal?.FindFirstValue("sub"),
            out var id
        )
            ? id
            : null;
    public bool IsSaasSuperAdmin => Principal?.HasClaim("saas_super_admin", "true") == true;
}
