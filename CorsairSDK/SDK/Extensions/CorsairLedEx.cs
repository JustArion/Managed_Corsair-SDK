namespace Dawn.Libs.Corsair.SDK.Extensions;

using LowLevel;

public static class CorsairLedEx
{
    public static CorsairLedPosition[] GetLedPositions(this CorsairLed led)
    {
        var (success, value) = led.TryGetLedPositions();
        if (!success)
            throw new Exception(CorsairExtensions.ERROR_PROPERTYINFORMATION);

        return value;
    }
}