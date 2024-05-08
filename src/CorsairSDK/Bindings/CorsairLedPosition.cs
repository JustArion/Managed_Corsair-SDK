namespace Corsair.Bindings;

public partial struct CorsairLedPosition
{
    [NativeTypeName("CorsairLedLuid")]
    public uint id;

    public double cx;

    public double cy;
}
