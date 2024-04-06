using System.Data.SQLite;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace WbExtensions.Infrastructure.Database;

public static class InfrastructureDatabaseExtensions
{
    public static IServiceCollection SetupDatabase(this IServiceCollection services)
    {
        using (var connection = new SQLiteConnection($"Data Source={GetDatabasePath()};"))
        {
            connection.Open();
        }

        return services;
    }

    private static string GetDatabasePath()
    {
        var databaseName = "wbextensions.db";
        var baseDirectory = Directory.GetCurrentDirectory();
        var parentPath = Directory.GetParent(baseDirectory)!.FullName;
        var dbFolder = Path.Combine(parentPath, "db");
        if (!Directory.Exists(dbFolder))
        {
            Directory.CreateDirectory(dbFolder);
        }
        return Path.Combine(dbFolder, databaseName);
    }
}