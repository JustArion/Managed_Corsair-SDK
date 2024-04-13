namespace Dawn.CorsairSDK.Rewrite.Device.Devices;

using System.Diagnostics;

public class Headset : CorsairDevice
{
    private int _batteryLevel;

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

        WirelessHeadsetOptions = new HeadsetOptions
        {
            BatteryUpdateIntervalSeconds = 1
        };
    }

    public bool SurroundSoundEnabled { get; }
    public bool SidetoneEnabled { get; }
    public int EqualizerPreset { get; }

    public HeadsetOptions WirelessHeadsetOptions { get; }

    private int _lastBatteryUpdate;

    private bool BatteryLevelNeedsUpdate() => (Environment.TickCount - _lastBatteryUpdate) * 1000 <
                                              WirelessHeadsetOptions.BatteryUpdateIntervalSeconds;
    /// <summary>
    /// 100% if the device has no battery
    /// </summary>
    public int BatteryLevel
    {
        get
        {
            if (!BatteryLevelNeedsUpdate())
                return _batteryLevel;
            
            _lastBatteryUpdate = Environment.TickCount;
            _batteryLevel = _interop.ReadDeviceProperty(Id, DeviceProperty.BatteryLevel).AsT1;

            return _batteryLevel;
        }
        private set => _batteryLevel = value;
    }

    public bool MicEnabled { get; }

    public const int MIN_BATTERY_LEVEL = 0;
    public const int MAX_BATTERY_LEVEL = 100;
}

public record HeadsetOptions
{
    public int BatteryUpdateIntervalSeconds { get; set; }
}
