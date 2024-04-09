using System;
using System.IO;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Domain;
using WbExtensions.Infrastructure.Database.Repositories;
using WbExtensions.Infrastructure.Database.Settings;
using WbExtensions.Infrastructure.Database.TableFactories;
using WbExtensions.Infrastructure.Database.TypeHandlers;

namespace WbExtensions.Infrastructure.Database;

internal static class InfrastructureDatabaseExtensions
{
    public static IServiceCollection SetupDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        SqlMapper.RemoveTypeMap(typeof(DateTime));
        SqlMapper.AddTypeHandler(new DateTimeHandler());

        var databaseSettings = configuration.GetSection("Database").Get<DatabaseSettings>()
                               ?? throw new ArgumentNullException(nameof(DatabaseSettings));

        var connectionString = GetConnectionString();

        services
            .AddSingleton(databaseSettings)
            .AddSingleton(new DbConnectionFactory(connectionString))
            .AddSingleton<ITableFactory<Telemetry>, TelemetryTableFactory>()
            .AddSingleton<ITelemetryRepository, TelemetryRepository>();

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
        
        return $"Data Source={dbPath};Pooling=True;Max Pool Size=100;Journal Mode=Off;";
    }
}