using Corsair.Lighting.Contracts;
using Corsair.Lighting.Internal;

namespace Corsair.Device.Devices;

public class Keyboard : CorsairDevice
{
    internal Keyboard(DeviceInformation deviceInformation) : base(deviceInformation)
    {
        KeyboardLighting = new KeyboardLighting(CorsairSDK._connectionHandler, this);

        if (SupportedFeatures.Contains(DeviceProperty.LogicalLayout))
            PhysicalLayout = (PhysicalLayout)_interop.ReadDeviceProperty(Id, DeviceProperty.PhysicalLayout).AsT1;

        if (SupportedFeatures.Contains(DeviceProperty.PhysicalLayout))
            LogicalLayout = (LogicalLayout)_interop.ReadDeviceProperty(Id, DeviceProperty.LogicalLayout).AsT1;
    }

    public PhysicalLayout PhysicalLayout { get; }

    public LogicalLayout LogicalLayout { get; }

    public IKeyboardLighting KeyboardLighting { get; }
}
