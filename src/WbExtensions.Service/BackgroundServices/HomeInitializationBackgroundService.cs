using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Home;

namespace WbExtensions.Service.BackgroundServices;

internal sealed class HomeInitializationBackgroundService : BackgroundService
{
    private readonly IDevicesRepository _devicesRepository;
    private readonly ILogger<HomeInitializationBackgroundService> _logger;

    public HomeInitializationBackgroundService(
        IDevicesRepository devicesRepository,
        ILogger<HomeInitializationBackgroundService> logger)
    {
        _devicesRepository = devicesRepository;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _devicesRepository.InitAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации виртуальных устройств");
        }
    }
}