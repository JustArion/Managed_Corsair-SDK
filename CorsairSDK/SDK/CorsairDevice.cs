namespace Dawn.Libs.Corsair.SDK;

using System.Runtime.InteropServices;
using LowLevel;
using OneOf;

public static class CorsairDevice
{
    public static (bool Success, IEnumerable<CorsairDevicePropertyId> SupportedProperties) TryGetSupportedProperties(this CorsairDeviceInfo info)
    {
        CorsairSDK.EnsureConnected();
        
        OneOf<bool, int, double, string, bool[], int[], double[], string[]> property;
        try
        {
            property = ReadDeviceProperty(info, CorsairDevicePropertyId.CDPI_PropertyArray);
        }
        catch (Exception)
        {
            return (false, Enumerable.Empty<CorsairDevicePropertyId>());
        }


        return (true, property.AsT5.Select(x => (CorsairDevicePropertyId)x));
    }
    public static (bool Success, int BatteryLevel) TryGetBatteryLevel(this CorsairDeviceInfo info)
    {
        CorsairSDK.EnsureConnected();

        
        OneOf<bool, int, double, string, bool[], int[], double[], string[]> property;
        try
        {
            
            property = ReadDeviceProperty(info, CorsairDevicePropertyId.CDPI_BatteryLevel);
        }
        catch (Exception)
        {
            return (false, default);
        }

        

        return (true, property.AsT1);
    }
    
    public static (bool Success, bool Enabled) TryGetIsMicEnabled(this CorsairDeviceInfo info)
    {
        CorsairSDK.EnsureConnected();

        
        OneOf<bool, int, double, string, bool[], int[], double[], string[]> property;
        try
        {
            
            property = ReadDeviceProperty(info, CorsairDevicePropertyId.CDPI_MicEnabled);
        }
        catch (Exception)
        {
            return (false, default);
        }


        return (true, property.AsT0);
    }
    
    public static (bool Success, bool Enabled) TryGetIsSurroundSoundEnabled(this CorsairDeviceInfo info)
    {
        CorsairSDK.EnsureConnected();

        
        OneOf<bool, int, double, string, bool[], int[], double[], string[]> property;
        try
        {
            
            property = ReadDeviceProperty(info, CorsairDevicePropertyId.CDPI_SurroundSoundEnabled);
        }
        catch (Exception)
        {
            return (false, default);
        }


        return (true, property.AsT0);
    }
    
    public static (bool Success, bool SidetoneEnabled) TryGetIsSidetoneEnabled(this CorsairDeviceInfo info)
    {
        CorsairSDK.EnsureConnected();

        
        OneOf<bool, int, double, string, bool[], int[], double[], string[]> property;
        try
        {
            
            property = ReadDeviceProperty(info, CorsairDevicePropertyId.CDPI_SidetoneEnabled);
        }
        catch (Exception)
        {
            return (false, default);
        }


        return (true, property.AsT0);
    }
    
    public static (bool Success, bool SidetoneEnabled) TryGetIsEqualizerPresent(this CorsairDeviceInfo info)
    {
        CorsairSDK.EnsureConnected();

        
        OneOf<bool, int, double, string, bool[], int[], double[], string[]> property;
        try
        {
            
            property = ReadDeviceProperty(info, CorsairDevicePropertyId.CDPI_EqualizerPreset);
        }
        catch (Exception)
        {
            return (false, default);
        }


        return (true, property.AsT0);
    }

    public static unsafe (bool Success, (CorsairDataType DataType, CorsairPropertyFlag Flags)) TryGetPropertyInfo(this CorsairDeviceInfo info, CorsairDevicePropertyId propertyId)
    {

        var dataType = default(CorsairDataType);
        var flags = default(CorsairPropertyFlag);

        var error = Methods.CorsairGetDevicePropertyInfo(info.id, propertyId, 0, &dataType, (uint*)&flags);

        return error != CorsairError.CE_Success 
            ? (false, default) 
            : (true, (dataType, flags));
    }


    public static unsafe OneOf<bool, int, double, string, bool[], int[], double[], string[]> ReadDeviceProperty(
        this CorsairDeviceInfo device, CorsairDevicePropertyId propertyId)
    {
        CorsairSDK.EnsureConnected();

        var property = default(CorsairProperty);

        Methods.CorsairReadDeviceProperty(device.id, propertyId, 0, &property).ThrowIfNecessary();

        try
        {
            switch (property.type)
            {
                case CorsairDataType.CT_Boolean:
                    return Convert.ToBoolean(property.value.boolean);
                case CorsairDataType.CT_Int32:
                    return property.value.int32;
                case CorsairDataType.CT_Float64:
                    return property.value.float64;
                case CorsairDataType.CT_String:
                    return ((nint)property.value.@string).ToAnsiString();
                case CorsairDataType.CT_Boolean_Array:
                    var boolArray = property.value.boolean_array;
                    return InteropHelper.ToManagedArray(boolArray.items, boolArray.count);
                case CorsairDataType.CT_Int32_Array:
                    var intArray = property.value.int32_array;
                    
                    var ia = new int[intArray.count];
                    Marshal.Copy((nint)intArray.items, ia, 0, (int)intArray.count);
                    return ia;
                case CorsairDataType.CT_Float64_Array:
                    var float64Array = property.value.float64_array;

                    var fa = new double[float64Array.count];
                    Marshal.Copy((nint)float64Array.items, fa, 0, (int)float64Array.count);

                    return fa;
                case CorsairDataType.CT_String_Array:
                    var stringArray = property.value.string_array;
                    return InteropHelper.ToManagedArray(stringArray.items, stringArray.count);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        finally
        {
            Methods.CorsairFreeProperty(&property);
        }
    }
}