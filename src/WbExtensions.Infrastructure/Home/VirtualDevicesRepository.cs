using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Domain.Home;
using WbExtensions.Infrastructure.Json;

namespace WbExtensions.Infrastructure.Home;

internal sealed class VirtualDevicesRepository : IVirtualDevicesRepository
{
    private readonly ITelemetryRepository _telemetryRepository;

    private readonly ConcurrentBag<VirtualDevice> _virtualDevices;
    private bool _inited;
    private readonly ManualResetEvent _manualResetEvent;

    public VirtualDevicesRepository(
        ITelemetryRepository telemetryRepository)
    {
        _telemetryRepository = telemetryRepository;
        _virtualDevices = new ConcurrentBag<VirtualDevice>();
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

    public async Task InitAsync(CancellationToken cancellationToken)
    {
        if(!_inited)
        {
            var devices = await ReadSchemaAsync(cancellationToken);
            var telemetryValues = await _telemetryRepository.GetAsync(cancellationToken);
            
            foreach (var device in devices!)
            {
                foreach (var control in device.Controls)
                {
                    var telemetry = telemetryValues.FirstOrDefault(t =>
                        t.Device == device.VirtualDeviceName
                        && t.Control == control.VirtualControlName);

                    if (telemetry is not null)
                    {
                        control.UpdateValue(telemetry.Value);
                    }
                }
                _virtualDevices.Add(device);
            }

            _inited = true;
        }

        _manualResetEvent.Set();
    }

    private async Task<IReadOnlyCollection<VirtualDevice>?> ReadSchemaAsync(CancellationToken cancellationToken)
    {
        var fileName = "schema.json";
        var baseDirectory = Directory.GetCurrentDirectory();
        var parentPath = Directory.GetParent(baseDirectory)!.FullName;
        var schemaFolder = Path.Combine(parentPath, "schema");
        if (!Directory.Exists(schemaFolder))
        {
            Directory.CreateDirectory(schemaFolder);
        }
        var filePath = Path.Combine(schemaFolder, fileName);

        await using var stream = File.OpenRead(filePath);
        
        return await JsonSerializer.DeserializeAsync<List<VirtualDevice>>(stream,
            new JsonSerializerOptions().Configure(),
            cancellationToken);
    }
}