using System.Text.Json;

namespace Dawn.CorsairSDK.Rewrite.Lighting.Contracts;

using System.Drawing;
using Rewrite;

public interface IEffectController : IDisposable
{
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys);
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys);
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKey[] keys);
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys);
    EffectReceipt FlickerZones(PulseInfo pulseInfo, KeyboardZones zones);

    EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys);
    EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys);
    EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKey[] keys);
    EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys);
    EffectReceipt PulseZones(PulseInfo pulseInfo, KeyboardZones zones);

    EffectReceipt FlashKeys(FlashInfo pulseInfo, params KeyboardKeys[] keys);
    EffectReceipt FlashKeys(FlashInfo pulseInfo, IEnumerable<KeyboardKeys> keys);
    EffectReceipt FlashKeys(FlashInfo pulseInfo, params KeyboardKey[] keys);
    EffectReceipt FlashKeys(FlashInfo pulseInfo, IEnumerable<KeyboardKey> keys);
    EffectReceipt FlashKeys(FlashInfo pulseInfo, KeyboardZones zones);



    void StopEffectOnKeys(params KeyboardKeys[] keys);
    void StopEffectOnKeys(params KeyboardKey[] keys);
    void StopEffectOnZones(KeyboardZones zones);
}


/// <param name="Start">The initial color the pulse should start from</param>
/// <param name="End">The color the effect will transition to (and then transition back to the Start)</param>
/// <param name="TotalDuration">The total duration of the animation, can be disregarded if IsInfinite is set to true</param>
/// <param name="Interval">The full interval that it takes Start -> End -> Start (0 -> 1T)</param>
/// <param name="IsInfinite">If the animation should not have an end duration, it will loop the interval</param>
/// <param name="PulseModulation">Controls the animation & wave of the pulse</param>
public record PulseInfo(Color Start, Color End, TimeSpan Interval, bool IsInfinite, TimeSpan TotalDuration = default)
{
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

public delegate double WaveFunction(float x);

public record FlashInfo(Color Color, TimeSpan FlashInterval, TimeSpan FlashDuration);
