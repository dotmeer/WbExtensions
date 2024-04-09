using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace WbExtensions.Infrastructure.Database.TableFactories;

internal abstract class TableFactoryBase<TModel> : ITableFactory<TModel>
{
    private readonly DbConnectionFactory _connectionFactory;

    private bool _executed;

    protected TableFactoryBase(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Migrate()
    {
        if(!_executed)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            MigrateInternalAsync(cts.Token).GetAwaiter().GetResult();

            _executed = true;
        }
    }

    protected IDbConnection GetConnection() => _connectionFactory.Create();

    protected abstract Task MigrateInternalAsync(CancellationToken cancellationToken);
}