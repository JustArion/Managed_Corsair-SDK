
using System.Diagnostics;
using Dawn.CorsairSDK.Bindings;
using Dawn.CorsairSDK.Rewrite.Connection;
using Dawn.CorsairSDK.Rewrite.Device.Internal.Contracts;
using Dawn.CorsairSDK.Rewrite.Tracing;

namespace Dawn.CorsairSDK.Rewrite.Device.Internal;

internal unsafe class DeviceConnection : IDeviceConnection
{
    private readonly int _connectionId;
    public DeviceConnection()
    {
        _connectionId = DeviceConnectionResolver.GetNewId(this);

        Debug.WriteLine($"New Device Connection[{_connectionId}] created", "Device Connections");
        SessionStateChanged += (_, e) => CurrentState = e;
    }

    ~DeviceConnection() => DeviceConnectionResolver.RemoveConnection(_connectionId);

    public bool Connect()
    {
        // If this shows an error in the IDE, it's lying. It compiles!
        // It's because of the collection expression on the attribute
        var result = Track.Interop(Interop.Connect(&DeviceConnectionResolver.DeviceStateChangeNativeCallback, (void*)_connectionId), _connectionId);


        return result == CorsairError.CE_Success;
    }

    public CorsairSessionState CurrentState { get; private set; }

    public EventHandler<CorsairSessionState>? SessionStateChanged { get; set; }

    public CorsairSessionDetails GetConnectionDetails()
    {
        var details = default(CorsairSessionDetails);
        Track.Interop(Interop.GetSessionDetails(&details), details).ThrowIfNecessary();
        return details;
    }

    public void Disconnect() => Track.Interop(Interop.Disconnect()).ThrowIfNecessary();


}
