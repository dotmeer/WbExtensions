using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Home;

namespace WbExtensions.Service.BackgroundServices;

internal sealed class InitializationBackgroundService : BackgroundService
{
    private readonly ILogger<InitializationBackgroundService> _logger;
    private readonly IDatabaseMigrator _databaseMigrator;
    private readonly IVirtualDevicesRepository _virtualDevicesRepository;

    public InitializationBackgroundService(
        ILogger<InitializationBackgroundService> logger,
        IDatabaseMigrator databaseMigrator,
        IVirtualDevicesRepository virtualDevicesRepository)
    {
        _logger = logger;
        _databaseMigrator = databaseMigrator;
        _virtualDevicesRepository = virtualDevicesRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _databaseMigrator.InitAsync(stoppingToken);
            _logger.LogInformation("Database inited");
            await _virtualDevicesRepository.InitAsync(stoppingToken);
            _logger.LogInformation("Device manager inited");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации");
        }
    }
}