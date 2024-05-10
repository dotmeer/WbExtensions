using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Alice;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Requests;

namespace WbExtensions.Application.Implementations.Alice;

internal sealed class InitializationFacade : IAliceDevicesManager
{
    private readonly AliceDevicesManager _manager;
    private readonly ManualResetEvent _manualResetEvent;

    public InitializationFacade(AliceDevicesManager manager)
    {
        _manager = manager;
        _manualResetEvent = new ManualResetEvent(false);
    }

    public async Task InitAsync(CancellationToken cancellationToken)
    {
        await _manager.InitAsync(cancellationToken);
        
        // TODO: проверить, что блокировка работает корректно - юнит-тесты?
        _manualResetEvent.Set();
    }

    public Task<IList<Device>> GetAsync(CancellationToken cancellationToken)
    {
        if (!_manualResetEvent.WaitOne(TimeSpan.FromMinutes(1)))
        {
            throw new SynchronizationLockException("Устройства все еще заблокированы");
        }

        return _manager.GetAsync(cancellationToken);
    }

    public Task<IList<Device>> GetAsync(IReadOnlyCollection<string> ids, CancellationToken cancellationToken)
    {
        if (!_manualResetEvent.WaitOne(TimeSpan.FromMinutes(1)))
        {
            throw new SynchronizationLockException("Устройства все еще заблокированы");
        }

        return _manager.GetAsync(ids, cancellationToken);
    }

    public Task<IList<Device>> UpdateDevicesStateAsync(IReadOnlyCollection<SetUSerDevicesStateRequestItem> actions, CancellationToken cancellationToken)
    {
        if (!_manualResetEvent.WaitOne(TimeSpan.FromMinutes(1)))
        {
            throw new SynchronizationLockException("Устройства все еще заблокированы");
        }

        return _manager.UpdateDevicesStateAsync(actions, cancellationToken);
    }
}