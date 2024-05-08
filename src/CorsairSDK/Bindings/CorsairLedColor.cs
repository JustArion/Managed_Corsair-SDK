namespace Corsair.Bindings;

public partial struct CorsairLedColor
{
    [NativeTypeName("CorsairLedLuid")]
    public uint id;

    [NativeTypeName("unsigned char")]
    public byte r;

    [NativeTypeName("unsigned char")]
    public byte g;

    [NativeTypeName("unsigned char")]
    public byte b;

    [NativeTypeName("unsigned char")]
    public byte a;
}
