using Dapper;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain;

namespace WbExtensions.Infrastructure.Database.Migrations;

internal sealed class TelemetryMigration : IMigration
{
    public async Task MigrateAsync(IDbConnection connection, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
create table if not exists {nameof(Telemetry)} (
    {nameof(Telemetry.Device)} text not null,
    {nameof(Telemetry.Control)} text not null,
    {nameof(Telemetry.Value)} text not null,
    {nameof(Telemetry.Updated)} text not null,
    primary key ({nameof(Telemetry.Device)}, {nameof(Telemetry.Control)})
);
",
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}