using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;

namespace HospitalSaaS.Api.Tests;

public sealed class PermissionAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Key = "REPLACE-USING-USER-SECRETS-AT-LEAST-32-BYTES";
    private readonly HttpClient _client;

    public PermissionAuthorizationTests(WebApplicationFactory<Program> factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task Authenticated_user_without_permission_is_forbidden()
    {
        SetToken([]);
        var response = await _client.GetAsync($"/api/v1/patients/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task User_with_permission_reaches_tenant_filtered_handler()
    {
        SetToken([new Claim("permission", "Patients.View")]);
        var response = await _client.GetAsync($"/api/v1/patients/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private void SetToken(IEnumerable<Claim> additionalClaims)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new("tenant_id", Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, "test@example.com"),
        };
        claims.AddRange(additionalClaims);
        var token = new JwtSecurityToken(
            "HospitalSaaS",
            "HospitalSaaS.Api",
            claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
                SecurityAlgorithms.HmacSha256
            )
        );
        _client.DefaultRequestHeaders.Authorization = new(
            "Bearer",
            new JwtSecurityTokenHandler().WriteToken(token)
        );
    }
}
