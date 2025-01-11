using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Application.UseCases.GetStateInRoom;

internal sealed class GetStateInRoomHandler : IRequestHandler<GetStateInRoomRequest, string>
{
    private readonly IVirtualDevicesRepository _virtualDevicesRepository;

    public GetStateInRoomHandler(IVirtualDevicesRepository virtualDevicesRepository)
    {
        _virtualDevicesRepository = virtualDevicesRepository;
    }

    public Task<string> Handle(GetStateInRoomRequest request, CancellationToken cancellationToken)
    {
        var devices = _virtualDevicesRepository
            .GetDevices()
            .Where(d => d.Room == request.Room);

        var result = new StringBuilder()
            .AppendLine($"{request.Room}:");

        foreach (var device in devices)
        {
            foreach (var control in device.Controls)
            {
                var name = GetName(control, device);

                if (name is null)
                {
                    continue;
                }

                result.AppendLine($"{name} {GetValue(control)}{GetMeasurement(control)}");
            }
        }

        return Task.FromResult(result.ToString());
    }

    private static string? GetName(Control control, VirtualDevice device)
        => control.Type switch
        {
            ControlType.Co2 => "Углекислый газ",
            ControlType.Contact => "Дверь",
            ControlType.Humidity => "Влажность",
            ControlType.Illuminance => "Освещенность",
            ControlType.Temperature => "Температура",
            ControlType.Voc => "Летучие частицы",
            ControlType.Switch => device.Name,
            _ => null
        };

    private static string GetValue(Control control)
        => control.Type switch
        {
            ControlType.Contact => string.Equals(control.Value, "true", StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(control.Value, "1", StringComparison.OrdinalIgnoreCase)
                ? "закрыта"
                : "открыта",
            ControlType.Switch => control.Value == "1"
                ? "вкл."
                : "выкл.",
            _ => control.Value
        };

    private static string GetMeasurement(Control control)
        => control.Type switch
        {
            ControlType.Co2 => " ppm",
            ControlType.Humidity => "%",
            ControlType.Illuminance => " люкс",
            ControlType.Temperature => " \u00b0C",
            ControlType.Voc => " ppb",
            _ => ""
        };
}