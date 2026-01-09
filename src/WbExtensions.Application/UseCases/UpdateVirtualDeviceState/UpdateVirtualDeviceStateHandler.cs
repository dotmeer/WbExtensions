using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WbExtensions.Application.Helpers;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Application.Models.Notification;

namespace WbExtensions.Application.UseCases.UpdateVirtualDeviceState;

internal sealed class UpdateVirtualDeviceStateHandler : IRequestHandler<UpdateVirtualDeviceStateRequest>
{
    private readonly IVirtualDevicesRepository _virtualDevicesRepository;
    private readonly IMediator _mediator;

    public UpdateVirtualDeviceStateHandler(
        IVirtualDevicesRepository virtualDevicesRepository,
        IMediator mediator)
    {
        _virtualDevicesRepository = virtualDevicesRepository;
        _mediator = mediator;
    }

    public async Task Handle(
        UpdateVirtualDeviceStateRequest request,
        CancellationToken cancellationToken)
    {
        var (virtualDeviceName, virtualControlName) = TopicNameHelper.ParseDeviceControlTopic(request.Topic);

        if (_virtualDevicesRepository.TryGetControl(virtualDeviceName, virtualControlName, out var virtualDevice, out var control))
        {
            if (control!.Value != request.Value
                && request.Value is not null)
            {
                // сохраняем в оперативной памяти
                control.UpdateValue(request.Value!);

                // отправляем состояние в Яндекс
                await _mediator.Publish(new PushUpdateToYandexNotification(virtualDevice!, control), cancellationToken);
            }
        }

        // сохраняем в БД, пишем метрики для численных значений
        await _mediator.Publish(
            new ValueNotification(virtualDeviceName, virtualControlName, request.Value),
            cancellationToken);
    }
}