using System.Drawing;

namespace Corsair.Lighting;

/// <param name="Start">The initial color the pulse should start from</param>
/// <param name="End">The color the effect will transition to (and then transition back to the Start)</param>
/// <param name="TotalDuration">The total duration of the animation, can be disregarded if IsInfinite is set to true</param>
/// <param name="Interval">The full interval that it takes Start -> End -> Start (0 -> 1T)</param>
/// <param name="IsInfinite">If the animation should not have an end duration, it will loop the interval</param>
/// <param name="PulseModulation">Controls the animation & wave of the pulse</param>
public record PulseInfo
{
    public PulseInfo(Color start, Color end, TimeSpan interval)
    {
        Start = start;
        End = end;
        Interval = interval;
        IsInfinite = true;
    }

    public PulseInfo(Color start, Color end, TimeSpan interval, TimeSpan totalDuration)
    {
        Start = start;
        End = end;
        Interval = interval;
        IsInfinite = false;
        TotalDuration = totalDuration;
    }

    public Color Start { get; init; }

    public Color End { get; init; }

    public TimeSpan Interval { get; init; }

    public bool IsInfinite { get; init; }

    public TimeSpan TotalDuration { get; init; }

    /// <summary>
    /// On a graph, For the amplitude, 0 -> 1 on the Y-Axis is when the Start color will reach the End color. If the amplitude is higher than 1, say 2, 0 -> 1 would be the Start color -> End color 2 times around.
    /// If the amplitude is lower than the inverse color will be used.
    /// </summary>
    /// <param name="WaveFunction">Allows you to manipulate the wave shape of the pulse based on the time that's passed An example of a "Fade In-Fade Out" would be Sin(x * Pi)</param>
    public WaveFunction? WaveModulation;

    /// <summary>
    /// The function is executed when <see cref="WaveModulation"/> returns a value that is NaN (Not a Number).
    /// </summary>
    public WaveFunction? OnNan;
}
