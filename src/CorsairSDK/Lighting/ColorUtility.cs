using System.Drawing;

namespace Corsair.Lighting;

public static class ColorUtility
{
    private static readonly Lazy<Random> _colorRandom = new(() => new());

    public static Color RandomColor(byte? fixedA = 255, byte? fixedR = null, byte? fixedG = null, byte? fixedB = null) => Color.FromArgb(
            alpha: fixedA ?? (byte)_colorRandom.Value.Next(0, 255),
            red: fixedR ?? (byte)_colorRandom.Value.Next(0, 255),
            green: fixedG ?? (byte)_colorRandom.Value.Next(0, 255),
            blue: fixedB ?? (byte)_colorRandom.Value.Next(0, 255));
}
