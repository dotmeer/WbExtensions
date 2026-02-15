using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Mqtt;
using WbExtensions.Domain.Mqtt;

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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _mqttService.SubscribeAsync(
                    _connection,
                    _handler.HandleAsync,
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background service for '{Handler}' failed, retrying in 1 minute", typeof(THandler).Name);
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}