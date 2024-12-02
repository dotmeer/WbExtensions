using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Application.Interfaces.Telegram;

namespace WbExtensions.Service.BackgroundServices;

internal sealed class InitializationBackgroundService : BackgroundService
{
    private readonly ILogger<InitializationBackgroundService> _logger;
    private readonly IDatabaseMigrator _databaseMigrator;
    private readonly IVirtualDevicesRepository _virtualDevicesRepository;
    private readonly ITelegramService _telegramService;

    public InitializationBackgroundService(
        ILogger<InitializationBackgroundService> logger,
        IDatabaseMigrator databaseMigrator,
        IVirtualDevicesRepository virtualDevicesRepository,
        ITelegramService telegramService)
    {
        _logger = logger;
        _databaseMigrator = databaseMigrator;
        _virtualDevicesRepository = virtualDevicesRepository;
        _telegramService = telegramService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _databaseMigrator.InitAsync(stoppingToken);
            _logger.LogInformation("Database inited");

            await _virtualDevicesRepository.InitAsync(stoppingToken);
            _logger.LogInformation("Device manager inited");

            await _telegramService.InitAsync(stoppingToken);
            _logger.LogInformation("Telegram bot inited");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации");
        }
    }
}