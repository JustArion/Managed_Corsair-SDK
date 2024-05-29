using System.Drawing;
using Corsair.Lighting.Animations;

namespace Corsair;

public record ScanAnimationOptions(Color Color, StartingPosition StartingPosition = StartingPosition.LeftToRight, bool Fill = false)
{
    public bool IsVertical { get; set; }
}
