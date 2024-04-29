using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Domain;
using WbExtensions.Infrastructure.Database.Settings;

namespace WbExtensions.Infrastructure.Database.Repositories;

internal sealed class TelemetryRepository : ITelemetryRepository, IDisposable
{
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly DatabaseSettings _databaseSettings;
    private readonly SemaphoreSlim _semaphore;

    public TelemetryRepository(
        DbConnectionFactory dbConnectionFactory,
        DatabaseSettings databaseSettings)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _databaseSettings = databaseSettings;
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async Task UpsertAsync(Telemetry model, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            if ((!_databaseSettings.StorableDevices.Any()
                 || _databaseSettings.StorableDevices.Contains(model.Device, StringComparer.OrdinalIgnoreCase))
                && (!_databaseSettings.StorableControls.Any()
                    || _databaseSettings.StorableControls.Contains(model.Control, StringComparer.OrdinalIgnoreCase)))
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
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IReadOnlyCollection<Telemetry>> GetAsync(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        
        var result = new List<Telemetry>(0);

        try
        {
            var command = new CommandDefinition(
                $@"select * from {nameof(Telemetry)}",
                cancellationToken: cancellationToken);

            using var connection = _dbConnectionFactory.Create();

            result.AddRange(await connection.QueryAsync<Telemetry>(command));
        }
        finally
        {
            _semaphore.Release();
        }

        return result;
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}