using System.Drawing;
using System.Numerics;

namespace Dawn.CorsairSDK.Rewrite.Lighting.Internal;

internal static class ColorEffects
{
    internal static async Task FadeTo(Color start, Color end, TimeSpan pulseInterval, Action<Color> onLerp, CancellationToken token)
    {
        float progress = 0;
        var timeMs = Environment.TickCount;

        while (progress < 1 && !token.IsCancellationRequested)
        {
            progress = (float)((Environment.TickCount - timeMs) /
                               Math.Clamp(pulseInterval.TotalMilliseconds, 1, float.MaxValue));

            var lerpedColor = LerpColor(start, end, progress, 1);

            onLerp(lerpedColor);
            await Task.Delay(1, token);
        }
    }

    internal static Color LerpColor(Color start, Color end, float t, float amplitude)
    {
        // We have the start value, and the end value.
        // The progress is the % between those 2 values.

        t = Math.Clamp(t, 0, 1);

        // use case
        var lerpedA = Lerp(start.A, end.A, t);
        var lerpedR = Lerp(start.R, end.R, t);
        var lerpedG = Lerp(start.G, end.G, t);
        var lerpedB = Lerp(start.B, end.B, t);

        lerpedA *= amplitude;
        lerpedR *= amplitude;
        lerpedG *= amplitude;
        lerpedB *= amplitude;

        // lerpedA *= Math.Clamp(amplitude, 1, float.MaxValue);
        // lerpedR /= Math.Clamp(amplitude, 1, float.MaxValue);
        // lerpedG /= Math.Clamp(amplitude, 1, float.MaxValue);
        // lerpedB /= Math.Clamp(amplitude, 1, float.MaxValue);



        return Color.FromArgb(unchecked((byte)lerpedA), unchecked((byte)lerpedR), unchecked((byte)lerpedG), unchecked((byte)lerpedB));
    }

    private static float Lerp(float start, float end, double t) => (float)(start + ((end - start) * t));
}
