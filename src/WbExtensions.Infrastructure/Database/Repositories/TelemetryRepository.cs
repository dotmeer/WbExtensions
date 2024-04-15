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

internal sealed class TelemetryRepository : ITelemetryRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly DatabaseSettings _databaseSettings;

    public TelemetryRepository(
        DbConnectionFactory dbConnectionFactory,
        DatabaseSettings databaseSettings)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _databaseSettings = databaseSettings;
    }

    // TODO: смотреть, будут ли ошибки блокировки базы, если нет, то так и оставить
    public async Task UpsertAsync(Telemetry model, CancellationToken cancellationToken)
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

    public async Task<IReadOnlyCollection<Telemetry>> GetAsync(CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(
            $@"select * from {nameof(Telemetry)}",
            cancellationToken: cancellationToken);

        using var connection = _dbConnectionFactory.Create();

        var result = await connection.QueryAsync<Telemetry>(command);

        return result.ToList();
    }
}