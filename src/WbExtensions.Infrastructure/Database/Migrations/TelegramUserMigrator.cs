using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using WbExtensions.Domain;

namespace WbExtensions.Infrastructure.Database.Migrations;

internal sealed class TelegramUserMigrator : IMigration
{
    public async Task MigrateAsync(IDbConnection connection, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@$"
create table if not exists {nameof(TelegramUser)} (
    {nameof(TelegramUser.UserId)} integer not null,
    {nameof(TelegramUser.UserName)} text null,
    {nameof(TelegramUser.IsAllowed)} integer not null,
    {nameof(TelegramUser.IsAdmin)} integer not null,
    primary key ({nameof(TelegramUser.UserId)})
);
",
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}