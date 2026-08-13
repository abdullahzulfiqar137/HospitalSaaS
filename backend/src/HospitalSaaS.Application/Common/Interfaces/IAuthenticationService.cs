using HospitalSaaS.Application.Authentication.Models;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<Result<AuthTokens>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken
    );
    Task<Result<AuthTokens>> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task<Result> RevokeAsync(string refreshToken, CancellationToken cancellationToken);
    Task<Result> ChangePasswordAsync(
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken
    );
}
