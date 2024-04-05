using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WbExtensions.Domain.Alice.Capabilities.ColorSetting;
using WbExtensions.Domain.Alice.Capabilities.Mode;
using WbExtensions.Domain.Alice.Capabilities.OnOff;
using WbExtensions.Domain.Alice.Capabilities.Range;
using WbExtensions.Domain.Alice.Capabilities.Toggle;
using WbExtensions.Domain.Alice.Capabilities.VideoStream;

namespace WbExtensions.Domain.Alice.Constants;

public static class CapabilityStateInstances
{
    // on/off
    public const string On = "on";

    // collor settings
    public const string Hsv = "hsv";
    public const string Rgb = "rgb";
    public const string TemperatureK = "temperature_k";
    public const string Scene = "scene";

    // video stream
    public const string GetStream = "get_stream";

    // mode
    public const string CleanupMode = "cleanup_mode";
    public const string CoffeeMode = "coffee_mode";
    public const string Dishwashing = "dishwashing";
    public const string FanSpeed = "fan_speed";
    public const string Heat = "heat";
    public const string InputSource = "input_source";
    public const string Program = "program";
    public const string Swing = "swing";
    public const string TeaMode = "tea_mode";
    public const string Thermostat = "thermostat";
    public const string WorkSpeed = "work_speed";

    // range
    public const string Brightness = "brightness";
    public const string Channel = "channel";
    public const string Humidity = "humidity";
    public const string Open = "open";
    public const string Temperature = "temperature";
    public const string Volume = "volume";

    // toggle
    public const string Backlight = "backlight";
    public const string ControlsLocked = "controls_locked";
    public const string Ionization = "ionization";
    public const string KeepWarm = "keep_warm";
    public const string Mute = "mute";
    public const string Oscillation = "oscillation";
    public const string Pause = "pause";

    public static IDictionary<string, Type> InstanceCapabilityStateTypeMapping = new ConcurrentDictionary<string, Type>
    {
        [On] = typeof(OnOffCapabilityState),
        [Hsv] = typeof(HsvColorSettingCapabilityState),
        [Rgb] = typeof(RgbColorSettingCapabilityState),
        [TemperatureK] = typeof(TemperatureColorSettingCapabilityState),
        [Scene] = typeof(SceneColorSettingCapabilityState),
        [GetStream] = typeof(VideoStreamCapabilityState),
        [CleanupMode] = typeof(ModeCapabilityState),
        [CoffeeMode] = typeof(ModeCapabilityState),
        [Dishwashing] = typeof(ModeCapabilityState),
        [FanSpeed] = typeof(ModeCapabilityState),
        [Heat] = typeof(ModeCapabilityState),
        [InputSource] = typeof(OnOffCapabilityState),
        [Program] = typeof(ModeCapabilityState),
        [Swing] = typeof(ModeCapabilityState),
        [TeaMode] = typeof(ModeCapabilityState),
        [Thermostat] = typeof(ModeCapabilityState),
        [WorkSpeed] = typeof(ModeCapabilityState),
        [Brightness] = typeof(RangeCapabilityState),
        [Channel] = typeof(RangeCapabilityState),
        [Humidity] = typeof(RangeCapabilityState),
        [Open] = typeof(RangeCapabilityState),
        [Temperature] = typeof(RangeCapabilityState),
        [Volume] = typeof(RangeCapabilityState),
        [Backlight] = typeof(ToggleCapabilityState),
        [ControlsLocked] = typeof(ToggleCapabilityState),
        [Ionization] = typeof(ToggleCapabilityState),
        [KeepWarm] = typeof(ToggleCapabilityState),
        [Mute] = typeof(ToggleCapabilityState),
        [Oscillation] = typeof(ToggleCapabilityState),
        [Pause] = typeof(ToggleCapabilityState)
    };
}