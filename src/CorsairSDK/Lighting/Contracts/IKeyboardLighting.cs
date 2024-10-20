﻿using Corsair.Device.Devices;

namespace Corsair.Lighting.Contracts;

/// <summary>
/// Lighting and Effects for the keyboard
/// </summary>
public interface IKeyboardLighting
{

    /// <summary>
    /// Tries to connect the iCUE
    /// </summary>
    /// <param name="accessLevel">Shared by other programs / presets or exclusive control</param>
    /// <returns>Initialization succeeded</returns>
    bool TryInitialize(AccessLevel accessLevel = AccessLevel.Shared);

    IKeyboardColorController Colors { get; }

    IEffectController Effects { get; }

    Keyboard Device { get; }

    /// <summary>
    /// Shuts down all lighting on the keyboard and disconnects from iCUE
    /// </summary>
    void Shutdown();
}
