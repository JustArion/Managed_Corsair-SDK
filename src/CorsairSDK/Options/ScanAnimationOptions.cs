using System.Drawing;
using Corsair.Lighting.Animations;

namespace Corsair;

public record ScanAnimationOptions(Color Color, StartingPosition StartingPosition = StartingPosition.LeftToRight, bool Fill = false)
{
    public bool IsVertical { get; init; }

    public TimeSpan Duration { get; init; } = DefaultDuration;

    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(3);

}
