using System.Threading;
using System.Threading.Tasks;
using Dapper;
using WbExtensions.Domain;

namespace WbExtensions.Infrastructure.Database.TableFactories;

internal sealed class TelemetryTableFactory : TableFactoryBase<Telemetry>
{
    public TelemetryTableFactory(DbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    protected override async Task MigrateInternalAsync(CancellationToken cancellationToken)
    {
        await CreateTableAsync(cancellationToken);
    }

    private async Task CreateTableAsync(CancellationToken cancellationToken)
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

        using var connection = GetConnection();

        await connection.ExecuteAsync(command);
    }
}