using Corsair.Lighting.Animations.Internal;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Animations;

public sealed class VerticalScanAnimation(ScanAnimationOptions options, IKeyboardColorController colorController) : ScanAnimation(VSAOptionsFormatter.Format(options), colorController);

static file class VSAOptionsFormatter
{
    /// <summary>
    /// Sets the starting position to be vertical if not already
    /// </summary>
    internal static ScanAnimationOptions Format(ScanAnimationOptions options)
    {
        if (options.StartPosition is StartingPosition.LeftToRight or StartingPosition.RightToLeft)
            return options with { StartPosition = StartingPosition.TopToBottom };
        return options;
    }
}
