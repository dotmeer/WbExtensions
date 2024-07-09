using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Database;

namespace WbExtensions.Infrastructure.Database.Migrations;

internal sealed class DatabaseMigrator : IDatabaseMigrator
{
    private readonly DbConnectionFactory _connectionFactory;
    private bool _inited = false;

    public DatabaseMigrator(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitAsync(CancellationToken cancellationToken)
    {
        if (!_inited)
        {
            var migrations = Assembly.GetExecutingAssembly().GetTypes()
                .Where(_ => _.IsAssignableTo(typeof(IMigration)) && _.IsClass)
                .Select(_ => (IMigration)Activator.CreateInstance(_)!);

            foreach (var migration in migrations)
            {
                await migration.MigrateAsync(_connectionFactory.Create(), cancellationToken);
            }

            _inited = true;
        }
    }
}