using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Domain;
using WbExtensions.Infrastructure.Database.Settings;
using WbExtensions.Infrastructure.Database.TableFactories;

namespace WbExtensions.Infrastructure.Database.Repositories;

internal sealed class TelemetryRepository : ITelemetryRepository, IAsyncDisposable
{
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<TelemetryRepository> _logger;
    private readonly DatabaseSettings _databaseSettings;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ConcurrentQueue<Telemetry> _upsertQueue;
    private readonly Task _upsertTask;

    public TelemetryRepository(
        ITableFactory<Telemetry> tableFactory,
        DbConnectionFactory dbConnectionFactory,
        ILogger<TelemetryRepository> logger,
        DatabaseSettings databaseSettings)
    {
        tableFactory.Migrate();
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
        _databaseSettings = databaseSettings;
        _cancellationTokenSource = new CancellationTokenSource();
        _upsertQueue = new ConcurrentQueue<Telemetry>();
        _upsertTask = Task.Run(() => UpsertingTask(_cancellationTokenSource.Token));
    }

    public Task UpsertAsync(Telemetry model, CancellationToken cancellationToken)
    {
        if ((!_databaseSettings.StorableDevices.Any()
             || _databaseSettings.StorableDevices.Contains(model.Device, StringComparer.OrdinalIgnoreCase))
            && (!_databaseSettings.StorableControls.Any()
                || _databaseSettings.StorableControls.Contains(model.Control, StringComparer.OrdinalIgnoreCase)))
        {
            _upsertQueue.Enqueue(model);
        }

        return Task.CompletedTask;
    }

    public async Task<IReadOnlyCollection<Telemetry>> GetAsync(CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(
            $@"select * from {nameof(Telemetry)}",
            cancellationToken: cancellationToken);

        using var connection = _dbConnectionFactory.Create();

        var result = await connection.QueryAsync<Telemetry>(command);

        return result.ToList();
    }
    
    public async ValueTask DisposeAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        await _upsertTask;
        _cancellationTokenSource.Dispose();
    }

    private async Task UpsertingTask(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_upsertQueue.TryDequeue(out var telemetry))
                {
                    await UpsertInternalAsync(telemetry, cancellationToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing upsert queue");
            }
        }
    }

    private async Task UpsertInternalAsync(Telemetry model, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
insert into {nameof(Telemetry)} ({nameof(Telemetry.Device)}, {nameof(Telemetry.Control)}, {nameof(Telemetry.Value)}, {nameof(Telemetry.Updated)})
values(@{nameof(Telemetry.Device)}, @{nameof(Telemetry.Control)}, @{nameof(Telemetry.Value)}, @{nameof(Telemetry.Updated)})
on conflict ({nameof(Telemetry.Device)}, {nameof(Telemetry.Control)}) do update set
    {nameof(Telemetry.Value)} = @{nameof(Telemetry.Value)},
    {nameof(Telemetry.Updated)} = @{nameof(Telemetry.Updated)};",
            new
            {
                model.Device,
                model.Control,
                model.Value,
                model.Updated
            },
            cancellationToken: cancellationToken);

        using var connection = _dbConnectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}