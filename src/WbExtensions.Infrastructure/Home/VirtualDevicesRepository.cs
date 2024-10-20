using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Domain.Home;

namespace WbExtensions.Infrastructure.Home;

internal sealed class VirtualDevicesRepository : IVirtualDevicesRepository
{
    private readonly ITelemetryRepository _telemetryRepository;

    private readonly ConcurrentBag<VirtualDevice> _virtualDevices;
    private bool _inited;
    private readonly ManualResetEvent _manualResetEvent;

    public VirtualDevicesRepository(
        DevicesSchema schema, 
        ITelemetryRepository telemetryRepository)
    {
        _telemetryRepository = telemetryRepository;
        _virtualDevices = new ConcurrentBag<VirtualDevice>(schema.Devices);
        _manualResetEvent = new ManualResetEvent(false);
    }

    public IReadOnlyCollection<VirtualDevice> GetDevices()
    {
        if (!_manualResetEvent.WaitOne(TimeSpan.FromMinutes(1)))
        {
            throw new SynchronizationLockException("Устройства все еще заблокированы");
        }

        return _virtualDevices;
    }

    public bool TryGetControl(string virtualDeviceName, string virtualControlName, out VirtualDevice? virtualDevice, out Control? control)
    {
        if (!_manualResetEvent.WaitOne(TimeSpan.FromMinutes(1)))
        {
            throw new SynchronizationLockException("Устройства все еще заблокированы");
        }

        virtualDevice = _virtualDevices
            .FirstOrDefault(d => d.VirtualDeviceName == virtualDeviceName
                                 && d.Controls.Any(c => c.VirtualControlName == virtualControlName));

        control = virtualDevice?.Controls
            .FirstOrDefault(c => c.VirtualControlName == virtualControlName);

        return control is not null;
    }

    // TODO: насколько это нужно?
    public bool SetDeviceControlValue(string virtualDeviceName, string virtualControlName, string? value)
    {
        if (!_manualResetEvent.WaitOne(TimeSpan.FromMinutes(1)))
        {
            throw new SynchronizationLockException("Устройства все еще заблокированы");
        }

        if (TryGetControl(virtualDeviceName, virtualControlName, out var virtualDevice, out var control))
        {
            if (control!.Value != value
                && value is not null)
            {
                control.Value = value!;
                return true;
            }
        }

        return false;
    }

    public async Task InitAsync(CancellationToken cancellationToken)
    {
        if(!_inited)
        {
            var telemetryValues = await _telemetryRepository.GetAsync(cancellationToken);

            foreach (var device in _virtualDevices)
            {
                foreach (var control in device.Controls)
                {
                    var telemetry = telemetryValues.FirstOrDefault(t =>
                        t.Device == device.VirtualDeviceName
                        && t.Control == control.VirtualControlName);

                    if (telemetry is not null)
                    {
                        control.Value = telemetry.Value;
                    }
                }
            }

            _inited = true;
        }

        _manualResetEvent.Set();
    }
}