namespace Dawn.CorsairSDK.Rewrite.Device.Devices;

using System.Diagnostics;

public class Headset : CorsairDevice
{
    public bool GetIsSurroundSoundEnabled()
    {
        const DeviceProperty PROPERTY = DeviceProperty.SurroundSoundEnabled;
        if (SupportedFeatures.Contains(PROPERTY))
            return _interop.ReadDeviceProperty(Id, PROPERTY).AsT0;

        Debug.WriteLine($"An invocation occurred to an unsupported property: {PROPERTY}");
        return false;
    }

    public bool IsSidetoneEnabled()
    {
        const DeviceProperty PROPERTY = DeviceProperty.SidetoneEnabled;
        if (SupportedFeatures.Contains(PROPERTY))
            return _interop.ReadDeviceProperty(Id, PROPERTY).AsT0;

        Debug.WriteLine($"An invocation occurred to an unsupported property: {PROPERTY}");
        return false;
    }

    public bool GetIsEqualizerPresent()
    {
        const DeviceProperty PROPERTY = DeviceProperty.EqualizerPreset;
        if (SupportedFeatures.Contains(PROPERTY))
            return _interop.ReadDeviceProperty(Id, PROPERTY).AsT0;

        Debug.WriteLine($"An invocation occurred to an unsupported property: {PROPERTY}");
        return false;
    }

    private const int MIN_BATTERY_LEVEL = 0;
    private const int MAX_BATTERY_LEVEL = 100;
    public int GetBatteryLevel()
    {
        const DeviceProperty PROPERTY = DeviceProperty.BatteryLevel;
        if (SupportedFeatures.Contains(PROPERTY))
            return _interop.ReadDeviceProperty(Id, PROPERTY).AsT1;

        Debug.WriteLine($"An invocation occurred to an unsupported property: {PROPERTY}");
        return MAX_BATTERY_LEVEL;
    }

    public bool IsMicEnabled()
    {
        const DeviceProperty PROPERTY = DeviceProperty.MicEnabled;
        if (SupportedFeatures.Contains(PROPERTY))
            return _interop.ReadDeviceProperty(Id, PROPERTY).AsT0;

        Debug.WriteLine($"An invocation occurred to an unsupported property: {PROPERTY}");
        return false;
    }

    internal Headset(DeviceInformation deviceInformation) : base(deviceInformation)
    {
    }
}
