namespace Corsair.Bindings;

public unsafe partial struct CorsairDeviceConnectionStatusChangedEvent
{
    [NativeTypeName("CorsairDeviceId")]
    public fixed sbyte deviceId[128];

    public bool isConnected;
}
