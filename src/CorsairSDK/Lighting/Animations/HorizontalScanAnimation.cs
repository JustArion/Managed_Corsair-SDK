using System.Diagnostics;
using System.Drawing;
using Corsair.Lighting.Animations.Internal;
using Corsair.Lighting.Contracts;
using Corsair.Lighting.Internal;

namespace Corsair.Lighting.Animations;

public sealed class HorizontalScanAnimation(ScanAnimationOptions options, IKeyboardColorController colorController) : ScanAnimation(options, colorController);
