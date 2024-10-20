using System.Diagnostics;
using System.Drawing;
using Corsair.Lighting.Animations.Internal;
using Corsair.Lighting.Contracts;
using Corsair.Lighting.Internal;

namespace Corsair.Lighting.Animations;

/// <exception cref="T:Corsair.Exceptions.DeviceNotConnectedException">The device is not connected, the operation could not be completed</exception>
/// <exception cref="T:Corsair.Exceptions.CorsairException">An unexpected event happened, the device may have gotten disconnected</exception>
public sealed class HorizontalScanAnimation(ScanAnimationOptions options, IKeyboardColorController keyboardColors) : ScanAnimation(HSAOptionsFormatter.Format(options), keyboardColors)
{
    /// <summary>
    /// A horizontal scan animation using the first available Corsair keyboard
    /// </summary>
    public HorizontalScanAnimation(ScanAnimationOptions options) : this(options, CorsairSDK.KeyboardLighting.Colors) { }
}

static file class HSAOptionsFormatter
{
    /// <summary>
    /// Sets the starting position to be horizontal if not already
    /// </summary>
    internal static ScanAnimationOptions Format(ScanAnimationOptions options)
    {
        if (options.StartPosition is StartingPosition.TopToBottom or StartingPosition.BottomToTop)
            return options with { StartPosition = StartingPosition.LeftToRight };
        return options;
    }
}
