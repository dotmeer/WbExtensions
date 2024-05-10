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
    private readonly DatabaseSettings _databaseSettings;
    private readonly BaseRepository _baseRepository;

    public TelemetryRepository(
        DatabaseSettings databaseSettings,
        BaseRepository baseRepository)
    {
        _databaseSettings = databaseSettings;
        _baseRepository = baseRepository;
    }

    public Task UpsertAsync(Telemetry model, CancellationToken cancellationToken)
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

            return _baseRepository.ExecuteAsync(command);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Telemetry>> GetAsync(CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(
            $@"select * from {nameof(Telemetry)}",
            cancellationToken: cancellationToken);

        return _baseRepository.QueryAsync<Telemetry>(command);
    }
}