using HospitalSaaS.Application.Authentication;
using HospitalSaaS.Application.Authentication.Models;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Contracts.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/auth")]
public sealed class AuthController(IRequestDispatcher dispatcher, IConfiguration configuration) : ApiControllerBase
{
    private const string RefreshCookie = "__Host-hospital-refresh";
    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await dispatcher.Send<LoginCommand, Result<AuthTokens>>(
            new LoginCommand(request.Email, request.Password),
            cancellationToken
        );
        if (!result.IsSuccess)
            return FromError(result.Error!);
        SetRefreshCookie(result.Value!);
        return Ok(Map(result.Value!));
    }

    [AllowAnonymous, HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RefreshTokenRequest? request,
        CancellationToken cancellationToken
    )
    {
        var refreshToken = ResolveRefreshToken(request);
        if (string.IsNullOrWhiteSpace(refreshToken))
            return MissingRefreshToken();
        var result = await dispatcher.Send<RefreshTokenCommand, Result<AuthTokens>>(
            new RefreshTokenCommand(refreshToken),
            cancellationToken
        );
        if (!result.IsSuccess)
        {
            DeleteRefreshCookie();
            return FromError(result.Error!);
        }
        SetRefreshCookie(result.Value!);
        return Ok(Map(result.Value!));
    }

    [Authorize, HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RefreshTokenRequest? request,
        CancellationToken cancellationToken
    )
    {
        var refreshToken = ResolveRefreshToken(request);
        if (string.IsNullOrWhiteSpace(refreshToken))
            return MissingRefreshToken();
        return await RevokeAndDeleteCookie(refreshToken, cancellationToken);
    }

    [Authorize, HttpPost("revoke")]
    public Task<IActionResult> Revoke(
        RefreshTokenRequest request,
        CancellationToken cancellationToken
    ) => RevokeToken(request.RefreshToken, cancellationToken);

    [Authorize, HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await dispatcher.Send<ChangePasswordCommand, Result>(
            new ChangePasswordCommand(request.CurrentPassword, request.NewPassword),
            cancellationToken
        );
        return result.IsSuccess ? NoContent() : FromError(result.Error!);
    }

    [Authorize, HttpGet("me")]
    public IActionResult Me() =>
        Ok(
            new
            {
                userId = User.FindFirst("sub")?.Value,
                tenantId = User.FindFirst("tenant_id")?.Value,
                email = User.FindFirst("email")?.Value,
            }
        );

    private async Task<IActionResult> RevokeToken(
        string refreshToken,
        CancellationToken cancellationToken
    )
    {
        var result = await dispatcher.Send<RevokeRefreshTokenCommand, Result>(
            new RevokeRefreshTokenCommand(refreshToken),
            cancellationToken
        );
        return result.IsSuccess ? NoContent() : FromError(result.Error!);
    }

    private async Task<IActionResult> RevokeAndDeleteCookie(string refreshToken, CancellationToken ct)
    {
        var response = await RevokeToken(refreshToken, ct);
        DeleteRefreshCookie();
        return response;
    }

    private string? ResolveRefreshToken(RefreshTokenRequest? request) =>
        Request.Cookies.TryGetValue(RefreshCookie, out var cookie) && !string.IsNullOrWhiteSpace(cookie)
            ? cookie
            : request?.RefreshToken;

    private void SetRefreshCookie(AuthTokens tokens)
    {
        var days = configuration.GetValue("Jwt:RefreshTokenDays", 30);
        Response.Cookies.Append(
            RefreshCookie,
            tokens.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/api/v1/auth",
                MaxAge = TimeSpan.FromDays(days),
                IsEssential = true,
            }
        );
    }

    private void DeleteRefreshCookie() => Response.Cookies.Delete(
        RefreshCookie,
        new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/api/v1/auth",
        }
    );

    private BadRequestObjectResult MissingRefreshToken() => BadRequest(
        new
        {
            code = "Authentication.RefreshTokenRequired",
            message = "A refresh token cookie or request value is required.",
            correlationId = HttpContext.TraceIdentifier,
        }
    );

    private static LoginResponse Map(AuthTokens tokens) =>
        new(tokens.AccessToken, tokens.RefreshToken, tokens.AccessTokenExpiresAtUtc);
}
