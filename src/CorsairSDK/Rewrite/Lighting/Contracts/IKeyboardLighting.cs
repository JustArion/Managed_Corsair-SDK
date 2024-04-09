namespace Dawn.CorsairSDK.Rewrite.Lighting.Contracts;

/// <summary>
/// Lighting & Effects for the keyboard
/// </summary>
public interface IKeyboardLighting
{
    bool TryInitialize(AccessLevel accessLevel = AccessLevel.Exclusive);

    IColorController Colors { get; }

    IEffectController Effects { get; }

    void Shutdown();
}
