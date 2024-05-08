namespace Dawn.CorsairSDK.Rewrite.Device.Devices;

public class HeadsetStand : CorsairDevice
{
    private bool _micEnabled;
    private bool _surroundSoundEnabled;
    private int _equalizerPreset;

    internal HeadsetStand(DeviceInformation deviceInformation) : base(deviceInformation) => SyncProperties();


    protected override void UpdateProperties()
    {
        SyncProperties();

        base.UpdateProperties(); // Update the counter.
    }

    private void SyncProperties()
    {
        if (SupportedFeatures.Contains(DeviceProperty.MicEnabled))
            MicEnabled = _interop.ReadDeviceProperty(Id, DeviceProperty.MicEnabled).AsT0;

        if (SupportedFeatures.Contains(DeviceProperty.SurroundSoundEnabled))
            SurroundSoundEnabled  = _interop.ReadDeviceProperty(Id, DeviceProperty.SurroundSoundEnabled).AsT0;

        if (SupportedFeatures.Contains(DeviceProperty.EqualizerPreset))
            EqualizerPreset = _interop.ReadDeviceProperty(Id, DeviceProperty.EqualizerPreset).AsT1;
    }

    public bool MicEnabled
    {
        get => GetAndUpdateIfNecessary(ref _micEnabled);
        private set => _micEnabled = value;
    }

    public bool SurroundSoundEnabled
    {
        get => GetAndUpdateIfNecessary(ref _surroundSoundEnabled);
        private set => _surroundSoundEnabled = value;
    }

    public int EqualizerPreset
    {
        get => GetAndUpdateIfNecessary(ref _equalizerPreset);
        private set => _equalizerPreset = value;
    }

}
