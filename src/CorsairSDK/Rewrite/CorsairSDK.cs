using Dawn.CorsairSDK.Rewrite;
using Dawn.CorsairSDK.Rewrite.Connection.Internal;
using Dawn.CorsairSDK.Rewrite.Connection.Internal.Contracts;
using Dawn.CorsairSDK.Rewrite.Device;
using Dawn.CorsairSDK.Rewrite.Device.Internal;
using Dawn.CorsairSDK.Rewrite.Device.Internal.Contracts;
using Dawn.CorsairSDK.Rewrite.Lighting.Contracts;
using Dawn.CorsairSDK.Rewrite.Lighting.Internal;

namespace Dawn.Rewrite;


public static class CorsairSDK
{

    internal static readonly DeviceConnectionHandler _connectionHandler = new();
    internal static readonly DeviceInteropHandler _deviceInterop = new();

    public static readonly IKeyboardLighting KeyboardLighting = new KeyboardLighting(_connectionHandler);

    public static IEnumerable<CorsairDevice> GetDevices(DeviceType deviceFilter = DeviceType.All)
    {
        _connectionHandler.Connect(DeviceReconnectPolicy.Default);
        return _deviceInterop.GetDevices(deviceFilter);
    }
}
