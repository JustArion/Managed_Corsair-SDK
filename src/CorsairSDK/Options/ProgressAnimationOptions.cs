namespace Corsair.Lighting.Animations;

public readonly record struct ProgressAnimationOptions(
    KeyboardKeys[] Keys,
    StartingPosition StartPosition,
    byte InitialProgress = 0
    );

