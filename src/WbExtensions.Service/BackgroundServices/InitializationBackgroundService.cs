using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Alice;
using WbExtensions.Application.Interfaces.Database;

namespace WbExtensions.Service.BackgroundServices;

internal sealed class InitializationBackgroundService : BackgroundService
{
    private readonly ILogger<InitializationBackgroundService> _logger;
    private readonly IAliceDevicesManager _aliceDevicesManager;
    private readonly IDatabaseMigrator _databaseMigrator;

    public InitializationBackgroundService(
        ILogger<InitializationBackgroundService> logger,
        IAliceDevicesManager aliceDevicesManager,
        IDatabaseMigrator databaseMigrator)
    {
        _logger = logger;
        _aliceDevicesManager = aliceDevicesManager;
        _databaseMigrator = databaseMigrator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _databaseMigrator.InitAsync(stoppingToken);
            _logger.LogInformation("Database inited");
            await _aliceDevicesManager.InitAsync(stoppingToken);
            _logger.LogInformation("Device manager inited");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации");
        }
    }
}