namespace Dawn.CorsairSDK.Extensions;

using LowLevel;

public static class LedControllerEx
{
    public static CorsairLedPosition[] GetLedPositions(this LedController ledController)
    {
        var (success, value) = ledController.TryGetLedPositions();
        if (!success)
            throw new Exception(CorsairExtensions.ERROR_PROPERTYINFORMATION);

        return value;
    }

    public static CorsairLedColor[] GetLedColors(this LedController ledController)
    {
        var (success, value) = ledController.TryGetLedColors();
        if (!success)
            throw new Exception(CorsairExtensions.ERROR_PROPERTYINFORMATION);

        return value;
    }
    
    public static LedInformation[] GetLedInformation(this LedController ledController)
    {
        var (success, value) = ledController.TryGetLedInformation();
        if (!success)
            throw new Exception(CorsairExtensions.ERROR_PROPERTYINFORMATION);

        return value;
    }
}