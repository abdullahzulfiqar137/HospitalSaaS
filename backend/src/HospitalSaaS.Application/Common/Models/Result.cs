namespace HospitalSaaS.Application.Common.Models;

public class Result
{
    protected Result(bool success, Error? error)
    {
        IsSuccess = success;
        Error = error;
    }

    public bool IsSuccess { get; }
    public Error? Error { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(Error error) => new(false, error);
}
