using System.Threading.Channels;
using HospitalSaaS.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HospitalSaaS.Infrastructure.BackgroundJobs;

public sealed class BackgroundJobService(ILogger<BackgroundJobService> logger)
    : BackgroundService,
        IBackgroundJobService
{
    private readonly Channel<QueuedJob> _jobs = Channel.CreateBounded<QueuedJob>(
        new BoundedChannelOptions(1_000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
        }
    );

    public string Enqueue(Func<CancellationToken, Task> job)
    {
        ArgumentNullException.ThrowIfNull(job);
        var queued = new QueuedJob(Guid.NewGuid().ToString("N"), job);
        if (!_jobs.Writer.TryWrite(queued))
            throw new InvalidOperationException("The background job queue is full.");
        return queued.Id;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var queued in _jobs.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await queued.Work(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception exception)
            {
                logger.LogError(exception, "Background job {JobId} failed", queued.Id);
            }
        }
    }

    private sealed record QueuedJob(string Id, Func<CancellationToken, Task> Work);
}
