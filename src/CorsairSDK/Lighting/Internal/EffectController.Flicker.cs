using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Internal;

internal partial class EffectController
{
    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys)
        => PulseKeys(pulseInfo with { WaveModulation = CommonWaveModulations.Flicker }, keys);

    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys)
        => PulseKeys(pulseInfo with { WaveModulation = CommonWaveModulations.Flicker }, keys);
    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKey[] keys)
        => PulseKeys(pulseInfo with { WaveModulation = CommonWaveModulations.Flicker }, keys);

    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys)
        => PulseKeys(pulseInfo with { WaveModulation = CommonWaveModulations.Flicker }, keys);

    public EffectReceipt FlickerZones(PulseInfo pulseInfo, KeyboardZones zones)
        => PulseZones(pulseInfo with { WaveModulation = CommonWaveModulations.Flicker }, zones);
}
