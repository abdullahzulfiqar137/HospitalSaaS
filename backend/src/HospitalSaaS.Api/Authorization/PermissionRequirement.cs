using Microsoft.AspNetCore.Authorization;

namespace HospitalSaaS.Api.Authorization;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;
