using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HospitalSaaS.Application.Authentication.Models;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Users;
using HospitalSaaS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HospitalSaaS.Infrastructure.Authentication;

public sealed class AuthenticationService(
    HospitalDbContext db,
    IPasswordHasher<User> passwordHasher,
    IOptions<JwtOptions> options,
    ICurrentUser currentUser
) : IAuthenticationService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<Result<AuthTokens>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken
    )
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await db
            .Users.IgnoreQueryFilters()
            .Include(x => x.Roles)
                .ThenInclude(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);

        if (user is null)
        {
            user = await db.Users.IgnoreQueryFilters()
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Permissions)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (user is null)
            return Result<AuthTokens>.Failure(
                new("Auth.InvalidCredentials", "Invalid email or password.", ErrorType.Unauthorized)
            );

        return Result<AuthTokens>.Success(await IssueAsync(user, cancellationToken));
    }

    public async Task<Result<AuthTokens>> RefreshAsync(
        string token,
        CancellationToken cancellationToken
    )
    {
        var stored = await db.RefreshTokens.SingleOrDefaultAsync(
            x => x.TokenHash == Hash(token),
            cancellationToken
        );
        if (stored is null || !stored.IsValid(DateTime.UtcNow))
            return Result<AuthTokens>.Failure(
                new(
                    "Auth.InvalidRefreshToken",
                    "Refresh token is invalid or expired.",
                    ErrorType.Unauthorized
                )
            );

        var user = await db
            .Users.Include(x => x.Roles)
                .ThenInclude(x => x.Permissions)
            .SingleAsync(x => x.Id == stored.UserId, cancellationToken);
        stored.Revoke(DateTime.UtcNow);
        return Result<AuthTokens>.Success(await IssueAsync(user, cancellationToken));
    }

    public async Task<Result> RevokeAsync(string token, CancellationToken cancellationToken)
    {
        var stored = await db.RefreshTokens.SingleOrDefaultAsync(
            x => x.TokenHash == Hash(token),
            cancellationToken
        );
        if (stored is not null)
        {
            stored.Revoke(DateTime.UtcNow);
            await db.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        if (currentUser.UserId is not Guid userId)
            return Result.Failure(
                new("Auth.Unauthorized", "Authentication is required.", ErrorType.Unauthorized)
            );
        if (newPassword.Length < 12)
            return Result.Failure(
                new(
                    "Auth.WeakPassword",
                    "New password must be at least 12 characters.",
                    ErrorType.Validation
                )
            );

        var user = await db.Users.SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null || !PasswordMatches(user, currentPassword))
            return Result.Failure(
                new(
                    "Auth.InvalidPassword",
                    "Current password is incorrect.",
                    ErrorType.Unauthorized
                )
            );

        user.ChangePassword(passwordHasher.HashPassword(user, newPassword));
        var activeTokens = await db
            .RefreshTokens.Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);
        foreach (var activeToken in activeTokens)
            activeToken.Revoke(DateTime.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private bool PasswordMatches(User user, string password) =>
        passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password)
        != PasswordVerificationResult.Failed;

    private async Task<AuthTokens> IssueAsync(User user, CancellationToken cancellationToken)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("tenant_id", user.TenantId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
        };
        claims.AddRange(
            user.Roles.SelectMany(x => x.Permissions)
                .Select(x => x.Name)
                .Distinct()
                .Select(x => new Claim("permission", x))
        );
        if (user.Roles.Any(x => x.Name == "SaaS Super Admin"))
            claims.Add(new Claim("saas_super_admin", "true"));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var jwt = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        db.RefreshTokens.Add(
            new RefreshToken(
                user.TenantId,
                user.Id,
                Hash(refreshToken),
                DateTime.UtcNow.AddDays(_options.RefreshTokenDays)
            )
        );
        await db.SaveChangesAsync(cancellationToken);
        return new(new JwtSecurityTokenHandler().WriteToken(jwt), refreshToken, expiresAtUtc);
    }

    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
