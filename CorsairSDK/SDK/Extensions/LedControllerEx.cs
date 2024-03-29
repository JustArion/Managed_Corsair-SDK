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
    
    public static void SetLedColor(this LedController ledController, uint id, (byte R, byte G, byte B, byte A) colors) 
        => SetLedColor(ledController, new CorsairLedColor { id = id, r = colors.R, g = colors.G, b = colors.B, a = colors.A });
    public static void SetLedColor(this LedController ledController, CorsairLedPosition position, (byte R, byte G, byte B, byte A) colors) 
        => SetLedColor(ledController, new CorsairLedColor { id = position.id, r = colors.R, g = colors.G, b = colors.B, a = colors.A });

    public static void SetLedColor(this LedController ledController, LedInformation information)
    {
        var color = information.Color;
        if (color.id == default)
            color = color with { id = information.Position.id };

        SetLedColor(ledController, color);
    }

    public static void SetLedColor(this LedController ledController, uint id, CorsairLedColor color)
    {
        if (color.id != id)
            color = color with { id = id };

        SetLedColor(ledController, color);
    }

    public static void SetLedColor(this LedController ledController, CorsairLedPosition position, CorsairLedColor color)
    {
        if (color.id == default)
            color = color with { id = position.id };

        SetLedColor(ledController, color);
    }
    
    public static void SetLedColor(this LedController ledController, CorsairLedColor color)
    {
        if (color.id == default)
            throw new InvalidOperationException("Method requires an LED Id");

        var success = ledController.TrySetLedColor(color);

        if (!success)
            throw new Exception("Failed to set Led Color");
    }
    
    
    public static async Task SetLedColorsAsync(this LedController ledController, params (uint id, (byte R, byte G, byte B, byte A) RGBA)[] colors)
    {
        if (colors.Any(x => x.id == default))
            throw new InvalidOperationException("Method requires an LED Id");
        
        var success = await ledController.TrySetLedColorsAsync(colors);

        if (!success)
            throw new Exception("Failed to set Led Colors");
    }

    public static async Task SetLedColorsAsync(this LedController ledController, CorsairLedPosition[] position, (byte R, byte G, byte B, byte A)[] colors)
    {       
        var success = await ledController.TrySetLedColorsAsync(position, colors);

        if (!success)
            throw new Exception("Failed to set Led Colors");
    }

    public static async Task SetLedColorsAsync(this LedController ledController, params LedInformation[] information)
    {
        var success = await ledController.TrySetLedColorsAsync(information);

        if (!success)
            throw new Exception("Failed to set Led Colors");
    }
    public static async Task SetLedColorsAsync(this LedController ledController, params (uint Id, CorsairLedColor Color)[] Colors)
    {
        var success = await ledController.TrySetLedColorsAsync(Colors);

        if (!success)
            throw new Exception("Failed to set Led Colors");
    }
    public static async Task SetLedColorsAsync(this LedController ledController, CorsairLedPosition[] positions, CorsairLedColor[] colors)
    {
        var success = await ledController.TrySetLedColorsAsync(positions, colors);

        if (!success)
            throw new Exception("Failed to set Led Colors");
    }
    public static async Task SetLedColorsAsync(this LedController ledController, params CorsairLedColor[] colors)
    {
        var success = await ledController.TrySetLedColorsAsync(colors);

        if (!success)
            throw new Exception("Failed to set Led Colors");
    }
    
    public static async Task SetGlobalColorAsync(this LedController ledController, CorsairLedColor colors)
    {
        var success = await ledController.TrySetGlobalColorAsync(colors);

        if (!success)
            throw new Exception("Failed to set Led Colors");
    }
}