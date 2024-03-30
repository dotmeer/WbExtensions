using dotmeer.WbExtensions.Application.Jobs;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace dotmeer.WbExtensions.Service.BackgroundServices;

internal sealed class MqttTopicsMetricsBackgroundJob : BackgroundService
{
    private readonly MqttDevicesControlsMetricsJob _mqttDevicesControlsMetricsJob;

    public MqttTopicsMetricsBackgroundJob(MqttDevicesControlsMetricsJob mqttDevicesControlsMetricsJob)
    {
        _mqttDevicesControlsMetricsJob = mqttDevicesControlsMetricsJob;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _mqttDevicesControlsMetricsJob.ExecuteAsync(stoppingToken);
    }
}