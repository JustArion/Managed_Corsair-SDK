using System.Diagnostics.CodeAnalysis;

namespace Corsair.Lighting.Contracts;

using System.Drawing;

/// <exception cref="T:Corsair.Exceptions.CorsairException">An unexpected event happened, the device may have gotten disconnected</exception>
public interface IEffectController : IDisposable
{
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKey[] keys);
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys);
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKeyState[] keys);
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeyState> keys);
    EffectReceipt FlickerZones(PulseInfo pulseInfo, KeyboardZones zones);

    EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKey[] keys);
    EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys);
    EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKeyState[] keys);
    EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeyState> keys);
    EffectReceipt PulseZones(PulseInfo pulseInfo, KeyboardZones zones);

    EffectReceipt FlashKeys(FlashInfo flashInfo, params KeyboardKey[] keys);
    EffectReceipt FlashKeys(FlashInfo flashInfo, IEnumerable<KeyboardKey> keys);
    EffectReceipt FlashKeys(FlashInfo flashInfo, params KeyboardKeyState[] keys);
    EffectReceipt FlashKeys(FlashInfo flashInfo, IEnumerable<KeyboardKeyState> keys);

    EffectReceipt FlashZones(FlashInfo pulseInfo, KeyboardZones zones);

    void StopEffectsOn(params KeyboardKey[] keys);
    void StopEffectsOn(params KeyboardKeyState[] keys);
    void StopEffectsOn(KeyboardZones zones);
}
