namespace Dawn.CorsairSDK.Bindings;

public unsafe partial struct CorsairDataType_StringArray
{
    [NativeTypeName("char **")]
    public sbyte** items;

    [NativeTypeName("unsigned int")]
    public uint count;
}
