namespace HospitalSaaS.Application.Authentication.Models;

public sealed record AuthTokens(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc
);
