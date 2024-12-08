using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Infrastructure.Database.Repositories;
using WbExtensions.Infrastructure.Database.Settings;
using WbExtensions.Infrastructure.Database.TypeHandlers;

namespace WbExtensions.Infrastructure.Database;

internal static class InfrastructureDatabaseExtensions
{
    public static IServiceCollection SetupDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDapper();

        var databaseSettings = configuration.GetSection("Database").Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(nameof(DatabaseSettings));

        var connectionString = GetConnectionString();
        var connectionFactory = new DbConnectionFactory(connectionString);

        services
            .AddSingleton(databaseSettings)
            .AddSingleton<IDbConnection, SQLiteConnection>(_ => new SQLiteConnection(connectionString))
            .AddSingleton(connectionFactory)
            .AddSingleton<BaseRepository>()
            .AddSingleton<ITelemetryRepository, TelemetryRepository>()
            .AddSingleton<IUserInfoRepository, UserInfoRepository>()
            .AddSingleton<ITelegramUserRepository, TelegramUserRepository>();

        return services;
    }

    private static string GetConnectionString()
    {
        var databaseName = "wbextensions.db";
        var baseDirectory = Directory.GetCurrentDirectory();
        var parentPath = Directory.GetParent(baseDirectory)!.FullName;
        var dbFolder = Path.Combine(parentPath, "db");
        if (!Directory.Exists(dbFolder))
        {
            Directory.CreateDirectory(dbFolder);
        }
        var dbPath = Path.Combine(dbFolder, databaseName);
        
        return $"Data Source={dbPath};Pooling=True;Max Pool Size=100;Journal Mode=WAL;";
    }

    private static void SetupDapper()
    {
        SqlMapper.RemoveTypeMap(typeof(DateTime));
        SqlMapper.AddTypeHandler(new DateTimeHandler());
        SqlMapper.RemoveTypeMap(typeof(bool));
        SqlMapper.AddTypeHandler(new BoolHandler());
    }
}