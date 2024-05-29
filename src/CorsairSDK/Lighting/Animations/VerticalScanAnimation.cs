using Corsair.Lighting.Animations.Internal;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Animations;

public sealed class VerticalScanAnimation(ScanAnimationOptions options, IKeyboardColorController colorController) : ScanAnimation(options with { IsVertical = true }, colorController);
