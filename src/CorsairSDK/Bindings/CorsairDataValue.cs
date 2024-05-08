namespace Corsair.Bindings;

[StructLayout(LayoutKind.Explicit)]
public unsafe partial struct CorsairDataValue
{
    [FieldOffset(0)]
    public bool boolean;

    [FieldOffset(0)]
    public int int32;

    [FieldOffset(0)]
    public double float64;

    [FieldOffset(0)]
    [NativeTypeName("char *")]
    public sbyte* @string;

    [FieldOffset(0)]
    public CorsairDataType_BooleanArray boolean_array;

    [FieldOffset(0)]
    public CorsairDataType_Int32Array int32_array;

    [FieldOffset(0)]
    public CorsairDataType_Float64Array float64_array;

    [FieldOffset(0)]
    public CorsairDataType_StringArray string_array;
}
