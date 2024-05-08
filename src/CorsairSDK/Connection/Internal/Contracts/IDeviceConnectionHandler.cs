namespace Corsair.Connection.Internal.Contracts;

internal interface IDeviceConnectionHandler
{
    bool Connect(DeviceReconnectPolicy? reconnectPolicy = default);

    DeviceReconnectPolicy ReconnectPolicy { get; set; }
    // Wait till connected
    void Wait(CancellationToken token = default);

    ConnectionState ConnectionState { get; }

    void Disconnect();
}
