using Corsair.Lighting.Animations.Internal;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Animations;

/// <exception cref="T:Corsair.Exceptions.DeviceNotConnectedException">The device is not connected, the operation could not be completed</exception>
/// <exception cref="T:Corsair.Exceptions.CorsairException">An unexpected event happened, the device may have gotten disconnected</exception>
public sealed class VerticalScanAnimation(ScanAnimationOptions options, IKeyboardColorController colorController) : ScanAnimation(VSAOptionsFormatter.Format(options), colorController)
{
    /// <summary>
    /// A vertical scan animation using the first available Corsair keyboard
    /// </summary>
    public VerticalScanAnimation(ScanAnimationOptions options) : this(options, CorsairSDK.KeyboardLighting.Colors) { }
}

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
