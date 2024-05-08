namespace Corsair.Bindings;

public unsafe partial struct CorsairDeviceInfo
{
    public CorsairDeviceType type;

    [NativeTypeName("CorsairDeviceId")]
    public fixed sbyte id[128];

    [NativeTypeName("char[128]")]
    public fixed sbyte serial[128];

    [NativeTypeName("char[128]")]
    public fixed sbyte model[128];

    public int ledCount;

    public int channelCount;
}
