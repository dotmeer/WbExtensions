using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WbExtensions.Application.MqttHandlers;
using WbExtensions.Domain.Mqtt;
using WbExtensions.Infrastructure.Mqtt.Abstractions;

namespace WbExtensions.Service.BackgroundServices;

internal sealed class RunMqttHandlerBackgroundService<THandler> : BackgroundService
    where THandler : IMqttHandler
{
    private readonly IMqttService _mqttService;
    private readonly THandler _handler;
    private readonly QueueConnection _connection;

    public RunMqttHandlerBackgroundService(
        IMqttService mqttService,
        THandler handler,
        QueueConnection connection)
    {
        _mqttService = mqttService;
        _handler = handler;
        _connection = connection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _mqttService.SubscribeAsync(
            _connection,
            _handler.HandleAsync,
            stoppingToken);
    }
}