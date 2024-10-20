using System.Drawing;

namespace Corsair.Lighting.Animations;

public readonly record struct ProgressAnimationOptions(
    StartingPosition StartPosition,
    KeyboardKeys[] Keys,
    Color ShiftColor = default,
    Color PercentFlashColor = default,
    byte InitialProgress = 0
)
{
    public bool IsVertical { get; } = StartPosition is StartingPosition.TopToBottom or StartingPosition.BottomToTop;
}

