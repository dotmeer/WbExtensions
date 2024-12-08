using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WbExtensions.Infrastructure;

namespace WbExtensions.Service.BackgroundServices;

internal sealed class InitializationBackgroundService : BackgroundService
{
    private readonly ILogger<InitializationBackgroundService> _logger;
    private readonly IReadOnlyCollection<IInitializer> _initializers;

    public InitializationBackgroundService(
        ILogger<InitializationBackgroundService> logger,
        IEnumerable<IInitializer> initializers)
    {
        _logger = logger;
        _initializers = initializers.ToList();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var initializer in _initializers.OrderBy(i => i.Order))
        {
            try
            {
                await initializer.InitAsync(stoppingToken);
                _logger.LogInformation("{Initializer} inited.", initializer.Name);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при инициализации {Initializer}", initializer.Name);
            }
        }
    }
}