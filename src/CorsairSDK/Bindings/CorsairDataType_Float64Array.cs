namespace Dawn.CorsairSDK.Bindings;

public unsafe partial struct CorsairDataType_Float64Array
{
    public double* items;

    [NativeTypeName("unsigned int")]
    public uint count;
}
