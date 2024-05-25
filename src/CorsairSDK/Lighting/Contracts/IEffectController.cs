using System.Diagnostics.CodeAnalysis;

namespace Corsair.Lighting.Contracts;

using System.Drawing;

public interface IEffectController : IDisposable
{
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys);
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys);
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKey[] keys);
    EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys);
    EffectReceipt FlickerZones(PulseInfo pulseInfo, KeyboardZones zones);

    EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys);
    EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys);
    EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKey[] keys);
    EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys);
    EffectReceipt PulseZones(PulseInfo pulseInfo, KeyboardZones zones);

    EffectReceipt FlashKeys(FlashInfo flashInfo, params KeyboardKeys[] keys);
    EffectReceipt FlashKeys(FlashInfo flashInfo, IEnumerable<KeyboardKeys> keys);
    EffectReceipt FlashKeys(FlashInfo flashInfo, params KeyboardKey[] keys);
    EffectReceipt FlashKeys(FlashInfo flashInfo, IEnumerable<KeyboardKey> keys);

    EffectReceipt FlashZones(FlashInfo pulseInfo, KeyboardZones zones);

    void StopEffectsOn(params KeyboardKeys[] keys);
    void StopEffectsOn(params KeyboardKey[] keys);
    void StopEffectsOn(KeyboardZones zones);
}
