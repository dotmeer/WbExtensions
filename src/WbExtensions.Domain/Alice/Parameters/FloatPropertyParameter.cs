using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Parameters;

public class FloatPropertyParameter : PropertyParameter
{
    public string Unit { get; }

    private FloatPropertyParameter(string instance, string unit)
        : base(instance)
    {
        Unit = unit;
    }

    public static FloatPropertyParameter FloatAmperage()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatAmperage,
            FloatPropertyUnits.Ampere);
    }

    public static FloatPropertyParameter FloatBatteryLevel()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatBatteryLevel,
            FloatPropertyUnits.Percent);
    }

    public static FloatPropertyParameter FloatCo2Level()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatCo2Level,
            FloatPropertyUnits.Ppm);
    }

    public static FloatPropertyParameter FloatElectricityMeter()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatElectricityMeter,
            FloatPropertyUnits.KilowattHour);
    }

    public static FloatPropertyParameter FloatFoodLevel()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatFoodLevel,
            FloatPropertyUnits.Percent);
    }

    public static FloatPropertyParameter FloatGasMeter()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatGasMeter,
            FloatPropertyUnits.CubicMeter);
    }

    public static FloatPropertyParameter FloatHeatMeter()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatHeatMeter,
            FloatPropertyUnits.GigaCalorie);
    }

    public static FloatPropertyParameter FloatHumidity()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatHumidity,
            FloatPropertyUnits.Percent);
    }

    public static FloatPropertyParameter FloatIllumination()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatIllumination,
            FloatPropertyUnits.IlluminationLux);
    }

    public static FloatPropertyParameter FloatMeter()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatMeter,
            null!);
    }

    public static FloatPropertyParameter FloatPm1Density()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatPm1Density,
            FloatPropertyUnits.DensityMcgM3);
    }

    public static FloatPropertyParameter FloatPm25Density()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatPm25Density,
            FloatPropertyUnits.DensityMcgM3);
    }

    public static FloatPropertyParameter FloatPm10Density()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatPm10Density,
            FloatPropertyUnits.DensityMcgM3);
    }

    public static FloatPropertyParameter FloatPower()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatPower,
            FloatPropertyUnits.Watt);
    }

    public static FloatPropertyParameter FloatPressureAtm()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatPressure,
            FloatPropertyUnits.PressureAtm);
    }

    public static FloatPropertyParameter FloatPressurePascal()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatPressure,
            FloatPropertyUnits.PressurePascal);
    }

    public static FloatPropertyParameter FloatPressureBar()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatPressure,
            FloatPropertyUnits.PressureBar);
    }

    public static FloatPropertyParameter FloatPressureMmhg()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatPressure,
            FloatPropertyUnits.PressureMmhg);
    }

    public static FloatPropertyParameter FloatTemperatureCelsius()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatTemperature,
            FloatPropertyUnits.TemperatureCelsius);
    }

    public static FloatPropertyParameter FloatTemperatureKelvin()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatTemperature,
            FloatPropertyUnits.TemperatureKelvin);
    }

    public static FloatPropertyParameter FloatTvoc()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatTvoc,
            FloatPropertyUnits.DensityMcgM3);
    }

    public static FloatPropertyParameter FloatVoltage()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatVoltage,
            FloatPropertyUnits.Volt);
    }

    public static FloatPropertyParameter FloatWaterLevel()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatWaterLevel,
            FloatPropertyUnits.Percent);
    }

    public static FloatPropertyParameter Amperage()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatAmperage,
            FloatPropertyUnits.Ampere);
    }

    public static FloatPropertyParameter FloatWaterMeter()
    {
        return new FloatPropertyParameter(
            PropertyInstances.FloatWaterMeter,
            FloatPropertyUnits.CubicMeter);
    }
}