using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<RunMqttHandlerBackgroundService<THandler>> _logger;

    public RunMqttHandlerBackgroundService(
        IMqttService mqttService,
        THandler handler,
        QueueConnection connection,
        ILoggerFactory loggerFactory)
    {
        _mqttService = mqttService;
        _handler = handler;
        _connection = connection;
        _logger = loggerFactory.CreateLogger<RunMqttHandlerBackgroundService<THandler>>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _mqttService.SubscribeAsync(
                _connection,
                _handler.HandleAsync,
                stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Background service for '{Handler}' failed", typeof(THandler).Name);
        }
    }
}