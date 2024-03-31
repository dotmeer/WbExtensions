using System.Threading;
using System.Threading.Tasks;
using dotmeer.WbExtensions.Application.Jobs;
using Microsoft.Extensions.Hosting;

namespace dotmeer.WbExtensions.Service.BackgroundServices;

internal sealed class RunJobBackgroundService<TJob> : BackgroundService
    where TJob : IJob
{
    private readonly TJob _job;

    public RunJobBackgroundService(TJob job)
    {
        _job = job;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _job.ExecuteAsync(stoppingToken);
    }
}