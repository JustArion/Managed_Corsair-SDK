using System.Drawing;
using Corsair.Lighting.Animations;

namespace Corsair.Lighting.Animations;

public readonly record struct ScanAnimationOptions(
    Color Color,
    StartingPosition StartPosition = StartingPosition.LeftToRight,
    bool Fill = false
    )
{
    public TimeSpan Duration { get; init; } = DefaultDuration;

    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(3);

}
