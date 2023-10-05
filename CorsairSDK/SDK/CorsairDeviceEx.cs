﻿namespace Dawn.Libs.Corsair.SDK;

using LowLevel;

public static class CorsairDeviceEx
{
    private const string ERROR_PROPERTYINFORMATION = "Failed to get property information";
    public static bool IsMicEnabled(this CorsairDeviceInfo info)
    {
        var (success, value) = info.TryGetIsMicEnabled();
        if (!success)
            throw new Exception(ERROR_PROPERTYINFORMATION);

        return value;
    }
    
    public static bool IsSidetoneEnabled(this CorsairDeviceInfo info)
    {
        var (success, value) = info.TryGetIsSidetoneEnabled();
        if (!success)
            throw new Exception(ERROR_PROPERTYINFORMATION);

        return value;
    }
    
    public static bool IsEqualizerPresent(this CorsairDeviceInfo info)
    {
        var (success, value) = info.TryGetIsEqualizerPresent();
        if (!success)
            throw new Exception(ERROR_PROPERTYINFORMATION);

        return value;
    }
    
    public static int GetBatteryLevel(this CorsairDeviceInfo info)
    {
        var (success, value) = info.TryGetBatteryLevel();
        if (!success)
            throw new Exception(ERROR_PROPERTYINFORMATION);

        return value;
    }

    public static IEnumerable<CorsairDevicePropertyId> GetSupportedProperties(this CorsairDeviceInfo info)
    {
        var (success, value) = info.TryGetSupportedProperties();
        if (!success)
            throw new Exception(ERROR_PROPERTYINFORMATION);

        return value;
    }

    public static (CorsairDataType DataType, CorsairPropertyFlag Flags) GetPropertyInfo(this CorsairDeviceInfo info, CorsairDevicePropertyId propertyId)
    {
        var (success, value) = info.TryGetPropertyInfo(propertyId);
        if (!success)
            throw new Exception(ERROR_PROPERTYINFORMATION);

        return value;
    }

    public static unsafe string GetID(this CorsairDeviceInfo info) => ((nint)info.id).ToAnsiString();
    public static unsafe string GetModel(this CorsairDeviceInfo info) => ((nint)info.model).ToAnsiString();
    public static unsafe string GetSerial(this CorsairDeviceInfo info) => ((nint)info.serial).ToAnsiString();

}