using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace WbExtensions.Infrastructure.Database.Repositories;

internal sealed class BaseRepository : IDisposable
{
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly SemaphoreSlim _semaphore;

    public BaseRepository(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async Task ExecuteAsync(CommandDefinition command)
    {
        await _semaphore.WaitAsync(command.CancellationToken);

        try
        {
            using var connection = _dbConnectionFactory.Create();

            await connection.ExecuteAsync(command);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IReadOnlyCollection<T>> QueryAsync<T>(CommandDefinition command)
    {
        await _semaphore.WaitAsync(command.CancellationToken);

        try
        {
            using var connection = _dbConnectionFactory.Create();
            var result = await connection.QueryAsync<T>(command);

            return result.ToList();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task<T?> FindAsync<T>(CommandDefinition command)
    {
        await _semaphore.WaitAsync(command.CancellationToken);

        try
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QuerySingleOrDefaultAsync<T>(command);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}