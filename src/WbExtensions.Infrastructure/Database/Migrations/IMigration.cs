using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace WbExtensions.Infrastructure.Database.Migrations;

internal interface IMigration
{
    Task MigrateAsync(IDbConnection connection, CancellationToken cancellationToken);
}