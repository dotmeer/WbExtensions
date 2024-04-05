using System.Text.Json.Serialization;

namespace WbExtensions.Domain.Alice.Capabilities;

public sealed class CapabilityStateActionResult
{
    public string Status { get; }

    [JsonPropertyName("error_code")]
    public string? ErrorCode { get; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; }

    private CapabilityStateActionResult(string status, string? errorCode, string? errorMessage)
    {
        Status = status;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static CapabilityStateActionResult Success()
    => new CapabilityStateActionResult("DONE", null, null);

    public static CapabilityStateActionResult ErrorDoorOpen() 
    => new CapabilityStateActionResult("ERROR", "DOOR_OPEN", "Открыта дверца");

    public static CapabilityStateActionResult ErrorLidOpen()
        => new CapabilityStateActionResult("ERROR", "LID_OPEN", "Открыта крышка");

    public static CapabilityStateActionResult ErrorRemoteControlDisabled()
        => new CapabilityStateActionResult("ERROR", "REMOTE_CONTROL_DISABLED", "Удаленное управление устройством отключено");
    
    public static CapabilityStateActionResult ErrorNotEnoughWater()
        => new CapabilityStateActionResult("ERROR", "NOT_ENOUGH_WATER", "Недостаточно воды");
    
    public static CapabilityStateActionResult ErrorLowChargeError()
        => new CapabilityStateActionResult("ERROR", "LOW_CHARGE_LEVEL", "Низкий уровень заряда");
    
    public static CapabilityStateActionResult ErrorContainerFull()
        => new CapabilityStateActionResult("ERROR", "CONTAINER_FULL", "Контейнер полон");
    
    public static CapabilityStateActionResult ErrorContainerEmpty()
        => new CapabilityStateActionResult("ERROR", "CONTAINER_EMPTY", "Контейнер пуст");
    
    public static CapabilityStateActionResult ErrorDripTrayFull()
        => new CapabilityStateActionResult("ERROR", "DRIP_TRAY_FULL", "Сливной поддон полон");
    
    public static CapabilityStateActionResult ErrorDeviceStuck()
        => new CapabilityStateActionResult("ERROR", "DEVICE_STUCK", "Устройство застряло");
    
    public static CapabilityStateActionResult ErrorDEviceOff()
        => new CapabilityStateActionResult("ERROR", "DEVICE_OFF", "Устройство выключено");
    
    public static CapabilityStateActionResult ErrorFirmwareOutOfDate()
        => new CapabilityStateActionResult("ERROR", "FIRMWARE_OUT_OF_DATE", "Прошивка устарела");
    
    public static CapabilityStateActionResult ErrorNotEnoughDetergent()
        => new CapabilityStateActionResult("ERROR", "NOT_ENOUGH_DETERGENT", "Недостаточно моющего средства");

    public static CapabilityStateActionResult ErrorHumanInvolvementNeeded()
        => new CapabilityStateActionResult("ERROR", "HUMAN_INVOLVEMENT_NEEDED", "Требуется вмешательство человека");

    public static CapabilityStateActionResult ErrorDeviceUnreachable()
        => new CapabilityStateActionResult("ERROR", "DEVICE_UNREACHABLE", "Устройство недоступно");

    public static CapabilityStateActionResult ErrorDeviceBusy()
        => new CapabilityStateActionResult("ERROR", "DEVICE_BUSY", "Устройство занято");

    public static CapabilityStateActionResult ErrorInternalError()
        => new CapabilityStateActionResult("ERROR", "INTERNAL_ERROR", "Неизвестная внутренняя ошибка");

    public static CapabilityStateActionResult ErrorInvalidAction()
        => new CapabilityStateActionResult("ERROR", "INVALID_ACTION", "Недопустимое действие");

    public static CapabilityStateActionResult ErrorInvalidValue()
        => new CapabilityStateActionResult("ERROR", "INVALID_VALUE", "Недопустимое значение");

    public static CapabilityStateActionResult ErrorNotSupportedInCurrentMode()
        => new CapabilityStateActionResult("ERROR", "NOT_SUPPORTED_IN_CURRENT_MODE", "Не поддерживается в текущем режиме работы устройства");

    public static CapabilityStateActionResult ErrorAccountLinkingError()
        => new CapabilityStateActionResult("ERROR", "ACCOUNT_LINKING_ERROR", "Ошибка в OAuth2 токене пользователя");

    public static CapabilityStateActionResult ErrorDeviceNotFound()
        => new CapabilityStateActionResult("ERROR", "DEVICE_NOT_FOUND", "Устройство не найдено");
}