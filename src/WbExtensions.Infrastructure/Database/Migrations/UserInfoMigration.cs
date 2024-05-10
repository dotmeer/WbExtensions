using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using WbExtensions.Domain;

namespace WbExtensions.Infrastructure.Database.Migrations;

internal sealed class UserInfoMigration : IMigration
{
    public async Task MigrateAsync(IDbConnection connection, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition($@"
create table if not exists {nameof(UserInfo)} (
    {nameof(UserInfo.Id)} text not null,
    {nameof(UserInfo.Email)} text not null,
    {nameof(UserInfo.Updated)} text not null,
    primary key ({nameof(UserInfo.Id)})
);
",
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}