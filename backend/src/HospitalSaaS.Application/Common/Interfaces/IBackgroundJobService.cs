namespace HospitalSaaS.Application.Common.Interfaces;

public interface IBackgroundJobService
{
    string Enqueue(Func<CancellationToken, Task> job);
}
