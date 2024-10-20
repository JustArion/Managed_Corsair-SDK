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

    public static async Task FadeTo(Color start, Color end, TimeSpan pulseInterval, Action<Color> onLerp, CancellationToken token)
    {
        float progress = 0;
        var timeMs = Environment.TickCount;

        while (progress < 1 && !token.IsCancellationRequested)
        {
            progress = (float)((Environment.TickCount - timeMs) /
                               Math.Clamp(pulseInterval.TotalMilliseconds, 1, float.MaxValue));

            var lerpedColor = LerpColor(start, end, progress);

            onLerp(lerpedColor);
            await Task.Delay(1, token);
        }
    }

    public static Color LerpColor(Color start, Color end, float t)
    {
        // We have the start value, and the end value.
        // The progress is the % between those 2 values.

        t = Math.Clamp(t, 0, 1);

        var lerpedA = Lerp(start.A, end.A, t);
        var lerpedR = Lerp(start.R, end.R, t);
        var lerpedG = Lerp(start.G, end.G, t);
        var lerpedB = Lerp(start.B, end.B, t);

        return Color.FromArgb(unchecked((byte)lerpedA), unchecked((byte)lerpedR), unchecked((byte)lerpedG), unchecked((byte)lerpedB));
    }

    public static Color SlerpColor(Color start, Color end, float t)
    {
        // We have the start value, and the end value.
        // The progress is the % between those 2 values.

        t = Math.Clamp(t, 0, 1);

        var smoothT = (float)(0.5 - (0.5 * Math.Cos(t * Math.PI)));

        var lerpedA = Lerp(start.A, end.A, smoothT);
        var lerpedR = Lerp(start.R, end.R, smoothT);
        var lerpedG = Lerp(start.G, end.G, smoothT);
        var lerpedB = Lerp(start.B, end.B, smoothT);

        return Color.FromArgb(unchecked((byte)lerpedA), unchecked((byte)lerpedR), unchecked((byte)lerpedG), unchecked((byte)lerpedB));
    }

    public static float Lerp(float start, float end, double t) => (float)(start + ((end - start) * t));
}
