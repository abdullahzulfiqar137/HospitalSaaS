namespace HospitalSaaS.Contracts.Authentication;

public sealed record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);
