using System.Data;
using System.Data.SQLite;

namespace WbExtensions.Infrastructure.Database;

internal sealed class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new SQLiteConnection(_connectionString);
    }
}