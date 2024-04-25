namespace Dawn.CorsairSDK.Rewrite.Device.Devices;

public class Headset : CorsairDevice
{
    private int _batteryLevel;
    private int _lastPropertyUpdate;
    private bool _surroundSoundEnabled;
    private bool _sidetoneEnabled;
    private int _equalizerPreset;
    private bool _micEnabled;

    internal Headset(DeviceInformation deviceInformation) : base(deviceInformation)
    {
        UpdateProperties();

        HeadsetOptions = new HeadsetOptions
        {
            PropertyUpdateIntervalSeconds = 1
        };
    }

    private void UpdateProperties()
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

        _lastPropertyUpdate = Environment.TickCount;
    }

    public bool SurroundSoundEnabled
    {
        get
        {
            if (!PropertiesNeedUpdating())
                return _surroundSoundEnabled;

            UpdateProperties();

            return _surroundSoundEnabled;
        }
        private set => _surroundSoundEnabled = value;
    }

    public bool SidetoneEnabled
    {
        get
        {
            if (!PropertiesNeedUpdating())
                return _sidetoneEnabled;

            UpdateProperties();

            return _sidetoneEnabled;
        }
        private set => _sidetoneEnabled = value;
    }

    public int EqualizerPreset
    {
        get
        {
            if (!PropertiesNeedUpdating())
                return _equalizerPreset;

            UpdateProperties();

            return _equalizerPreset;
        }
        private set => _equalizerPreset = value;
    }

    public HeadsetOptions HeadsetOptions { get; }


    private bool PropertiesNeedUpdating() => (Environment.TickCount - _lastPropertyUpdate) * 1000 <
                                             HeadsetOptions.PropertyUpdateIntervalSeconds;
    /// <summary>
    /// 100% if the device has no battery
    /// </summary>
    public int BatteryLevel
    {
        get
        {
            if (!PropertiesNeedUpdating())
                return _batteryLevel;

            UpdateProperties();

            return _batteryLevel;
        }
        private set => _batteryLevel = value;
    }

    public bool MicEnabled
    {
        get
        {
            if (!PropertiesNeedUpdating())
                return _micEnabled;

            UpdateProperties();

            return _micEnabled;
        }
        private set => _micEnabled = value;
    }

    public const int MIN_BATTERY_LEVEL = 0;
    public const int MAX_BATTERY_LEVEL = 100;
}

public record HeadsetOptions
{
    /// <summary>
    /// If 0, properties won't be cached
    /// </summary>
    public uint PropertyUpdateIntervalSeconds { get; set; }
}
