using Dawn.CorsairSDK.Rewrite.Lighting.Contracts;
using Dawn.CorsairSDK.Rewrite.Lighting.Internal;

namespace Dawn.CorsairSDK.Rewrite.Device.Devices;

public class Keyboard : CorsairDevice
{
    internal Keyboard(DeviceInformation deviceInformation) : base(deviceInformation)
        => KeyboardLighting = new KeyboardLighting(Dawn.Rewrite.CorsairSDK._connectionHandler, this);


    public IKeyboardLighting KeyboardLighting { get; }
}
