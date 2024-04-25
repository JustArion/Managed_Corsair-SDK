namespace Dawn.CorsairSDK.Rewrite.Device.Internal.Contracts;

using Bindings;

internal interface IDeviceConnection
{
    bool Connect();

    CorsairSessionState CurrentState { get; }
    EventHandler<CorsairSessionState>? SessionStateChanged { get; set; }

    CorsairSessionDetails GetConnectionDetails();

    void Disconnect();
}
