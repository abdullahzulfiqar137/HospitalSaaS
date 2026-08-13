namespace HospitalSaaS.Application.Common.Models;

public sealed record Error(string Code, string Message, ErrorType Type);

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    BusinessRule,
    Unexpected,
}
