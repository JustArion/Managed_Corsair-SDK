namespace Dawn.CorsairSDK.Rewrite.Device.Devices;

using System.Diagnostics;

public class Headset : CorsairDevice
{
    internal Headset(DeviceInformation deviceInformation) : base(deviceInformation)
    {
        if (SupportedFeatures.Contains(DeviceProperty.SurroundSoundEnabled))
            SurroundSoundEnabled  = _interop.ReadDeviceProperty(Id, DeviceProperty.SurroundSoundEnabled).AsT0;

        if (SupportedFeatures.Contains(DeviceProperty.SidetoneEnabled))
            SidetoneEnabled = _interop.ReadDeviceProperty(Id, DeviceProperty.SidetoneEnabled).AsT0;

        if (SupportedFeatures.Contains(DeviceProperty.EqualizerPreset))
            EqualizerPreset = _interop.ReadDeviceProperty(Id, DeviceProperty.EqualizerPreset).AsT1;

        BatteryLevel = SupportedFeatures.Contains(DeviceProperty.BatteryLevel)
            ? _interop.ReadDeviceProperty(Id, DeviceProperty.BatteryLevel).AsT1
            : MAX_BATTERY_LEVEL;

        if (SupportedFeatures.Contains(DeviceProperty.MicEnabled))
            MicEnabled = _interop.ReadDeviceProperty(Id, DeviceProperty.MicEnabled).AsT0;
    }

    public bool SurroundSoundEnabled { get; }
    public bool SidetoneEnabled { get; }
    public int EqualizerPreset { get; }
    /// <summary>
    /// 100% if the device has no battery
    /// </summary>
    public int BatteryLevel { get; }
    public bool MicEnabled { get; }

    public const int MIN_BATTERY_LEVEL = 0;
    public const int MAX_BATTERY_LEVEL = 100;
}
