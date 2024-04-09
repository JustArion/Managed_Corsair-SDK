namespace Dawn.CorsairSDK.Rewrite.Device;

using Internal.Contracts;

public class CorsairDevice
{
    internal CorsairDevice(DeviceInformation deviceInformation)
    {
        Id = deviceInformation.Id;
        Model = deviceInformation.Model;
        Serial = deviceInformation.Serial;
        Type = deviceInformation.Type;
        ChannelCount = deviceInformation.ChannelCount;
        LedCount = deviceInformation.LedCount;
        SupportedFeatures = deviceInformation.SupportedProperties;

        _interop = deviceInformation.InteropLayer;
    }
    internal readonly IDeviceInterop _interop;

    public DeviceProperty[] SupportedFeatures { get; private set; }

    public bool HasFeature(DeviceProperty property) => SupportedFeatures.Contains(property);

    /// <summary>
    /// A unique device identifier
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The Device Model, sort of like a Name for the device.
    /// </summary>
    public string Model { get; }

    /// <summary>
    /// Device Serial Number, Can be empty if not available for the device
    /// </summary>
    public string Serial { get; }

    public DeviceType Type { get; }

    public int ChannelCount { get; }

    /// <summary>
    /// The amount of controllable LEDs on the device
    /// </summary>
    public int LedCount { get; }


    public T AsDevice<T>() where T : CorsairDevice
    {
        if (this is T device)
            return device;
        throw new InvalidCastException();
    }
}
