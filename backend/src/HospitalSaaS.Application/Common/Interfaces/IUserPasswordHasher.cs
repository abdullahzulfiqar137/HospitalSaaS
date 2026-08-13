using HospitalSaaS.Domain.Users;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IUserPasswordHasher
{
    string Hash(User user, string password);
}
