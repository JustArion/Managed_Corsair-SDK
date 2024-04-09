namespace Dawn.CorsairSDK.Bindings;

public unsafe partial struct CorsairDataType_BooleanArray
{
    public bool* items;

    [NativeTypeName("unsigned int")]
    public uint count;
}
