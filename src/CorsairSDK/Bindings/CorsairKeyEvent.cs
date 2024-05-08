namespace Corsair.Bindings;

public unsafe partial struct CorsairKeyEvent
{
    [NativeTypeName("CorsairDeviceId")]
    public fixed sbyte deviceId[128];

    public CorsairMacroKeyId keyId;

    public bool isPressed;
}
