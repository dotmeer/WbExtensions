using dotmeer.WbExtensions.Application.Jobs;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace dotmeer.WbExtensions.Service.BackgroundServices;

internal sealed class Zigbee2MqttBackgroundJob : BackgroundService
{
    private readonly ParseZigbee2MqttEventsJob _parseZigbee2MqttEventsJob;

    public Zigbee2MqttBackgroundJob(ParseZigbee2MqttEventsJob parseZigbee2MqttEventsJob)
    {
        _parseZigbee2MqttEventsJob = parseZigbee2MqttEventsJob;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _parseZigbee2MqttEventsJob.ExecuteAsync(stoppingToken);
    }
}