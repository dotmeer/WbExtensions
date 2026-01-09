using System.Data;
using Microsoft.Data.Sqlite;

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
        return new SqliteConnection(_connectionString);
    }
}