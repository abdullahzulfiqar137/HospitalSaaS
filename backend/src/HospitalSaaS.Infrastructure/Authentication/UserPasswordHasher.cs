using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace HospitalSaaS.Infrastructure.Authentication;

public sealed class UserPasswordHasher(IPasswordHasher<User> hasher) : IUserPasswordHasher
{
    public string Hash(User user, string password) => hasher.HashPassword(user, password);
}
