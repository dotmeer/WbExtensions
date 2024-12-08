using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace WbExtensions.Infrastructure.Database.Migrations;

internal sealed class DatabaseMigrator : IInitializer
{
    private readonly DbConnectionFactory _connectionFactory;
    private bool _inited = false;

    public DatabaseMigrator(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public string Name => "Database";
    public int Order => 0;

    public async Task InitAsync(CancellationToken cancellationToken)
    {
        if (!_inited)
        {
            var migrations = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IMigration)) && t.IsClass)
                .Select(t => (IMigration)Activator.CreateInstance(t)!);

            foreach (var migration in migrations)
            {
                await migration.MigrateAsync(_connectionFactory.Create(), cancellationToken);
            }

            _inited = true;
        }
    }
}