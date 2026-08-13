namespace HospitalSaaS.Contracts.Authentication;

public sealed record LoginRequest(string Email, string Password);
