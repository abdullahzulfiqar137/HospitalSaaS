using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HospitalSaaS.Api.Tests;

public sealed class SecurityBoundaryTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SecurityBoundaryTests(WebApplicationFactory<Program> factory) =>
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );

    [Fact]
    public async Task Tenant_owned_endpoint_requires_tenant_context()
    {
        var response = await _client.GetAsync("/api/v1/patients");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Tenant_context_does_not_replace_authentication()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/patients");
        request.Headers.Add("X-Tenant-ID", Guid.NewGuid().ToString());
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Correlation_id_is_returned()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("X-Correlation-ID", "test-correlation-id");
        var response = await _client.SendAsync(request);
        Assert.Equal(
            "test-correlation-id",
            response.Headers.GetValues("X-Correlation-ID").Single()
        );
    }

    [Fact]
    public async Task Unsafe_correlation_id_is_replaced()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("X-Correlation-ID", new string('x', 129));

        var response = await _client.SendAsync(request);
        var returned = response.Headers.GetValues("X-Correlation-ID").Single();

        Assert.NotEqual(new string('x', 129), returned);
        Assert.Equal(32, returned.Length);
    }

    [Fact]
    public async Task Invalid_login_returns_readable_validation_error()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email = "invalid", password = "" }
        );
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Validation.Failed", body);
    }
}
