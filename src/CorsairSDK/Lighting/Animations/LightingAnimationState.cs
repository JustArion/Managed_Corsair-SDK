using Corsair.Lighting.Animations.Internal;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Animations;

public record LightingAnimationState(int FrameTimeMS, DateTimeOffset StartTime, IAnimation State);
