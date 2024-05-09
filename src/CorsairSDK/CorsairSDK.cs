using Corsair.Connection.Internal;
using Corsair.Device;
using Corsair.Device.Internal;
using Corsair.Lighting.Contracts;
using Corsair.Lighting.Internal;

namespace Corsair;


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
