using System.Data;
using System.Threading.Tasks;
using System.Threading;

namespace WbExtensions.Infrastructure.Database.Migrations;

internal interface IMigration
{
    Task MigrateAsync(IDbConnection connection, CancellationToken cancellationToken);
}