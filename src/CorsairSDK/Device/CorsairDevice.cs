using System.Diagnostics.CodeAnalysis;
using Corsair.Lighting.Contracts;
using Corsair.Lighting.Internal;

namespace Corsair.Device;

using Internal.Contracts;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CorsairDevice
{
    internal CorsairDevice(in DeviceInformation deviceInformation)
    {
        Id = deviceInformation.Id;
        Model = deviceInformation.Model;
        Serial = deviceInformation.Serial;
        Type = deviceInformation.Type;
        ChannelCount = deviceInformation.ChannelCount;
        LedCount = deviceInformation.LedCount;
        SupportedFeatures = deviceInformation.SupportedProperties;

        _interop = deviceInformation.InteropLayer;

        DeviceOptions = new DeviceOptions { PropertyUpdateIntervalSeconds = 1 };

        // if (LedCount <= 0)
            // return;

        LightingInterop = new LightingInterop();
        LightingInterop.SetDeviceContext(this);
    }
    internal readonly IDeviceInterop _interop;

    public ILightingInterop LightingInterop { get; protected set; }


    public DeviceOptions DeviceOptions { get; }
    protected virtual void UpdateProperties() => _lastPropertyUpdate = Environment.TickCount;

    protected int _lastPropertyUpdate;
    protected bool PropertiesNeedUpdating() => (Environment.TickCount - _lastPropertyUpdate) * 1000 < DeviceOptions.PropertyUpdateIntervalSeconds;

    protected T GetAndUpdateIfNecessary<T>(ref T field)
    {
        if (!PropertiesNeedUpdating())
            return field;

        UpdateProperties();

        return field;
    }
    public DeviceProperty[] SupportedFeatures { get; }

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

    public bool IsDeviceType<T>() where T : CorsairDevice => this is T;
}

public record DeviceOptions
{
    /// <summary>
    /// If 0, properties won't be cached
    /// </summary>
    public uint PropertyUpdateIntervalSeconds { get; set; }
}
