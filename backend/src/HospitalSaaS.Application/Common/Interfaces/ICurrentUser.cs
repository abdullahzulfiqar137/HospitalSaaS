namespace HospitalSaaS.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid? UserId { get; }
    bool IsSaasSuperAdmin { get; }
}
