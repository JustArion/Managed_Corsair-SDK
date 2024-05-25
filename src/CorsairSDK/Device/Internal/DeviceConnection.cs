using Corsair.Bindings;
using Corsair.Connection;
using Corsair.Device.Internal.Contracts;
using Corsair.Tracing;
using CorsairError = Corsair.Bindings.CorsairError;
using CorsairSessionDetails = Corsair.Bindings.CorsairSessionDetails;
using CorsairSessionState = Corsair.Bindings.CorsairSessionState;

namespace Corsair.Device.Internal;

internal unsafe class DeviceConnection : IDeviceConnection
{
    private readonly int _connectionId;
    public DeviceConnection()
    {
        _connectionId = DeviceConnectionResolver.GetNewId(this);

        SessionStateChanged += (_, e) => CurrentState = e;
    }

    public bool Connect()
    {
        // If this shows an error in the IDE, it's lying. It compiles!
        // It's because of the collection expression on the attribute
        var result = Interop.Connect(&DeviceConnectionResolver.DeviceStateChangeNativeCallback, (void*)_connectionId);

        InteropTracing.Trace(result, _connectionId);


        return result == CorsairError.CE_Success;
    }

    public CorsairSessionState CurrentState { get; private set; }

    public EventHandler<CorsairSessionState>? SessionStateChanged { get; set; }

    public CorsairSessionDetails GetConnectionDetails()
    {
        var details = default(CorsairSessionDetails);

        var result = Interop.GetSessionDetails(&details);

        InteropTracing.Trace(result, details);

        result.ThrowIfNecessary();
        return details;
    }

    public void Disconnect()
    {
        var result = Interop.Disconnect();

        InteropTracing.Trace(result);

        result.ThrowIfNecessary();
    }

    ~DeviceConnection() => DeviceConnectionResolver.RemoveConnection(_connectionId);
}
