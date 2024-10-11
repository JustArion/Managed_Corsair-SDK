using Corsair.Connection.Internal;
using Corsair.Device;
using Corsair.Device.Internal;
using Corsair.Lighting.Contracts;
using Corsair.Lighting.Internal;

namespace Corsair;

/// <summary>
/// Programmatic interaction with iCUE and Corsair hardware
/// </summary>
public static class CorsairSDK
{

    internal static readonly DeviceConnectionHandler _connectionHandler = new();
    internal static readonly DeviceInteropHandler _deviceInterop = new();

    /// <summary>
    /// Your device's main Corsair Keyboard if any
    /// </summary>
    public static IKeyboardLighting KeyboardLighting { get; } = new KeyboardLighting(_connectionHandler);

    /// <exception cref="T:Corsair.Exceptions.CorsairException">An unexpected event happened, the device may have gotten disconnected</exception>
    public static IEnumerable<CorsairDevice> GetDevices(DeviceType deviceFilter = DeviceType.All)
    {
        _connectionHandler.Connect(DeviceReconnectPolicy.Default);
        return _deviceInterop.GetDevices(deviceFilter);
    }

    /// <exception cref="T:Corsair.Exceptions.CorsairException">An unexpected event happened, the device may have gotten disconnected</exception>
    public static T? GetDeviceAs<T>() where T : CorsairDevice
    {
        var device = GetDevices().FirstOrDefault(x => x.IsDeviceType<T>());

        return device?.AsDevice<T>();
    }
}
