using System.Diagnostics;
using Dawn.CorsairSDK.Bindings;
using Dawn.CorsairSDK.Rewrite.Device.Internal;
using Dawn.CorsairSDK.Rewrite.Device.Internal.Contracts;
using Dawn.CorsairSDK.Rewrite.Threading;

namespace Dawn.CorsairSDK.Rewrite.Connection;

internal static class DeviceConnectionResolver
{
    private static AtomicInteger _nextId;
    private static readonly Dictionary<int, WeakReference<IDeviceConnection>> _connections = new();

    internal static int GetNewId(IDeviceConnection connection)
    {
        var id = _nextId.IncrementAndGet();
        _connections.Add(id, new(connection));
        Debug.WriteLine($"New Device Connection[{id}] created", "Device Connections");

        return id;
    }
    internal static void RemoveConnection(int connectionId)
    {
        _connections.Remove(connectionId);
        Debug.WriteLine($"Device Connection[{connectionId}] removed", "Device Connections");
    }


    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static unsafe void DeviceStateChangeNativeCallback(void* context, CorsairSessionStateChanged* data)
    {
        var id = (int)context;

        if (!_connections.TryGetValue(id, out var reference))
            return;

        var state = data->state;
        if (reference.TryGetTarget(out var connection))
            connection.SessionStateChanged?.Invoke(connection, state);
        else
            _connections.Remove(id);
    }
}
