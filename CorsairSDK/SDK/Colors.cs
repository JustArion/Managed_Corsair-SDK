namespace Dawn.CorsairSDK;

using LowLevel;

public static class Colors
{
    private static readonly Lazy<Random> _colorRandom = new(() => new());
    public static CorsairLedColor RandomColor(uint id, byte? fixedR = null, byte? fixedG = null, byte? fixedB = null, byte? fixedA = 255)
    {
        return new CorsairLedColor
        {
            id = id,
            r = fixedR ?? (byte)_colorRandom.Value.Next(0, 255),
            g = fixedG ?? (byte)_colorRandom.Value.Next(0, 255),
            b = fixedB ?? (byte)_colorRandom.Value.Next(0, 255),
            a = fixedA ?? (byte)_colorRandom.Value.Next(0, 255),
        };
    }
    
    /// RGBA 255, 255, 255, 255
    public static CorsairLedColor White(uint id) => new()
    {
        id = id,
        r = 255,
        g = 255,
        b = 255,
        a = 255
    };
    /// RGBA 0, 0, 0, 255
    public static CorsairLedColor Black(uint id) => new()
    {
        id = id,
        r = 0,
        g = 0,
        b = 0,
        a = 255
    };
    /// RGBA 127, 127, 127, 255 | 7F7F7FFF
    public static CorsairLedColor Grey(uint id) => new()
    {
        id = id,
        r = 127,
        g = 127,
        b = 127,
        a = 255
    };
    /// RGBA 127, 127, 127, 255 | 7F7F7FFF
    public static CorsairLedColor Gray(uint id) => Grey(id);
    

    /// RGBA 255, 0, 0, 255 | FF0000FF
    public static CorsairLedColor Red(uint id) => new()
    {
        id = id,
        r = 255,
        g = 0,
        b = 0,
        a = 255
    };
    /// RGBA 0, 255, 0, 255 | 00FF00FF
    public static CorsairLedColor Green(uint id) => new()
    {
        id = id,
        r = 0,
        g = 255,
        b = 0,
        a = 255
    };
    /// RGBA 0, 0, 255, 255 | 0000FFFF
    public static CorsairLedColor Blue(uint id) => new()
    {
        id = id,
        r = 0,
        g = 0,
        b = 255,
        a = 255
    };
    
    /// RGBA 255, 0, 255, 255 | FF00FFFF
    public static CorsairLedColor Purple(uint id) => new() 
    {
        id = id,
        r = 255,
        g = 0,
        b = 255,
        a = 255
    };
    /// RGBA 255, 255, 0, 255 FFFF00FF
    public static CorsairLedColor Yellow(uint id) => new()
    {
        id = id,
        r = 255,
        g = 255,
        b = 0,
        a = 255
    };
    /// RGBA 0, 255, 255, 255 | 00FFFFFF
    public static CorsairLedColor Cyan(uint id) => new()
    {
        id = id,
        r = 0,
        g = 255,
        b = 255,
        a = 255
    };
    /// RGBA 0, 127, 255, 255 007FFFFF
    public static CorsairLedColor LightBlue(uint id) => new()
    {
        id = id,
        r = 0,
        g = 127,
        b = 255,
        a = 255
    };
    /// RGBA 0, 255, 127, 255 | 00FF7FFF
    public static CorsairLedColor LightGreen(uint id) => new()
    {
        id = id,
        r = 0,
        g = 255,
        b = 127,
        a = 255
    };
    /// RGBA 255, 255, 127, 255 | FFFF7FFF
    public static CorsairLedColor LightYellow(uint id) => new()
    {
        id = id,
        r = 255,
        g = 255,
        b = 127,
        a = 255
    };
    /// RGBA 255, 127, 0, 255 | FF7F00FF
    public static CorsairLedColor Orange(uint id) => new()
    {
        id = id,
        r = 255,
        g = 127,
        b = 0,
        a = 255
    };
    /// RGBA 255, 0, 127, 255 | FF007FFF
    public static CorsairLedColor Pink(uint id) => new()
    {
        id = id,
        r = 255,
        g = 0,
        b = 127,
        a = 255
    };
}