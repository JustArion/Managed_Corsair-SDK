using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Corsair.Lighting;

// Flash Interval:  [------------[Flash Duration]]
// Effect Duration: [------------[Flash Duration]][------------[Flash Duration]] (For 2 flashes)
/// <param name="Color">The color of the flash</param>
/// <param name="FlashDuration">The amount of time it takes for 1 flash to complete</param>
/// <param name="IsInfinite">The total duration of the effect is infinite.</param>
/// <param name="EffectDuration">The total duration of the effect. The amount of flashes is the EffectDuration / SingleFlashDuration</param>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public readonly struct FlashInfo
{
    /// <summary>
    /// An infinite flashing effect
    /// </summary>
    /// <param name="color"></param>
    /// <param name="flashDuration"></param>
    public FlashInfo(Color color, TimeSpan flashDuration)
    {
        Color = color;
        FlashDuration = flashDuration;
        IsInfinite = true;
        EffectDuration = default;
    }

    /// <summary>
    /// A finite flashing effect determined by the <see cref="EffectDuration"/> property
    /// </summary>
    /// <param name="color"></param>
    /// <param name="flashDuration"></param>
    /// <param name="effectDuration"></param>
    public FlashInfo(Color color, TimeSpan flashDuration, TimeSpan effectDuration)
    {
        Color = color;
        FlashDuration = flashDuration;
        EffectDuration = effectDuration;
        IsInfinite = false;
    }

    /// <summary>
    /// A finite flashing effect determined by the <see cref="EffectDuration"/> property
    /// </summary>
    /// <param name="color"></param>
    /// <param name="flashDuration"></param>
    /// <param name="effectDuration"></param>
    public FlashInfo(Color color, TimeSpan flashDuration, int flashes) : this(color, flashDuration, flashDuration * flashes)
    {
    }

    public Color Color { get; }

    public TimeSpan FlashDuration { get; }

    public bool IsInfinite { get; }

    public bool UseSmoothFlashes { get; init; } = true;

    public TimeSpan EffectDuration { get; }

    public static readonly FlashInfo Default = new(Color.White, TimeSpan.FromMilliseconds(500));
}
