using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Alice;

namespace WbExtensions.Service.BackgroundServices;

internal sealed class HomeInitializationBackgroundService : BackgroundService
{
    private readonly IAliceDevicesManager _aliceDevicesManager;
    private readonly ILogger<HomeInitializationBackgroundService> _logger;

    public HomeInitializationBackgroundService(
        IAliceDevicesManager aliceDevicesManager,
        ILogger<HomeInitializationBackgroundService> logger)
    {
        _aliceDevicesManager = aliceDevicesManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _aliceDevicesManager.InitAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации виртуальных устройств");
        }
    }
}