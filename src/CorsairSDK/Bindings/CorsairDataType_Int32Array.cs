namespace Corsair.Bindings;

public unsafe partial struct CorsairDataType_Int32Array
{
    public int* items;

    [NativeTypeName("unsigned int")]
    public uint count;
}
