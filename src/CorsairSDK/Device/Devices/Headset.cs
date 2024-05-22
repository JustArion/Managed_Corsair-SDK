namespace Corsair.Device.Devices;

public class Headset : CorsairDevice
{
    private int _batteryLevel = -1;
    private bool _surroundSoundEnabled;
    private bool _sidetoneEnabled;
    private int _equalizerPreset = -1;
    private bool _micEnabled;

    internal Headset(DeviceInformation deviceInformation) : base(deviceInformation)
    {
        IsWireless = SupportedFeatures.Contains(DeviceProperty.BatteryLevel);
        SyncProperties();
    }

    protected override void UpdateProperties()
    {
        SyncProperties();

        base.UpdateProperties(); // Update the counter
    }

    private void SyncProperties()
    {
        if (SupportedFeatures.Contains(DeviceProperty.MicEnabled))
            MicEnabled = _interop.ReadDeviceProperty(Id, DeviceProperty.MicEnabled).AsT0;

        if (SupportedFeatures.Contains(DeviceProperty.SurroundSoundEnabled))
            SurroundSoundEnabled  = _interop.ReadDeviceProperty(Id, DeviceProperty.SurroundSoundEnabled).AsT0;

        if (SupportedFeatures.Contains(DeviceProperty.SidetoneEnabled))
            SidetoneEnabled = _interop.ReadDeviceProperty(Id, DeviceProperty.SidetoneEnabled).AsT0;

        if (SupportedFeatures.Contains(DeviceProperty.EqualizerPreset))
            EqualizerPreset = _interop.ReadDeviceProperty(Id, DeviceProperty.EqualizerPreset).AsT1;

        BatteryLevel = SupportedFeatures.Contains(DeviceProperty.BatteryLevel)
            ? _interop.ReadDeviceProperty(Id, DeviceProperty.BatteryLevel).AsT1
            : MAX_BATTERY_LEVEL;
    }

    public bool SurroundSoundEnabled
    {
        get => GetAndUpdateIfNecessary(ref _surroundSoundEnabled);
        private set => _surroundSoundEnabled = value;
    }

    public bool SidetoneEnabled
    {
        get => GetAndUpdateIfNecessary(ref _sidetoneEnabled);
        private set => _sidetoneEnabled = value;
    }

    public int EqualizerPreset
    {
        get => GetAndUpdateIfNecessary(ref _equalizerPreset);
        private set => _equalizerPreset = value;
    }

    public bool IsWireless { get; }
    /// <summary>
    /// 100% if the device has no battery
    /// </summary>
    public int BatteryLevel
    {
        get => GetAndUpdateIfNecessary(ref _batteryLevel);
        private set => _batteryLevel = value;
    }

    public bool MicEnabled
    {
        get => GetAndUpdateIfNecessary(ref _micEnabled);
        private set => _micEnabled = value;
    }

    public const int MIN_BATTERY_LEVEL = 0;
    public const int MAX_BATTERY_LEVEL = 100;
}
