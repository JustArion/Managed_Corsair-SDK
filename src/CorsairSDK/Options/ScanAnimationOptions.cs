using System.Drawing;
using Corsair.Lighting.Animations;

namespace Corsair.Lighting.Animations;

public readonly record struct ScanAnimationOptions(
    Color Color,
    StartingPosition StartPosition = StartingPosition.LeftToRight,
    bool Fill = false
    )
{
    public bool IsVertical { get; } = StartPosition is StartingPosition.TopToBottom or StartingPosition.BottomToTop;
    public TimeSpan Duration { get; init; } = DefaultDuration;

    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(3);

}
