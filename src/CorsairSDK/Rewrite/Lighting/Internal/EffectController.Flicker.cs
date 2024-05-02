using Dawn.CorsairSDK.Rewrite.Lighting.Contracts;

namespace Dawn.CorsairSDK.Rewrite.Lighting.Internal;

internal partial class EffectController
{
    // This creates a flicker pattern every 1.5T, the flicker is logaritmic
    /// <summary>
    /// sin(  log( tan(x) ) * pi^( sin( x^2 )  )
    /// </summary>
    private static readonly WaveFunction _flickerModulation = x =>
        Math.Sin(
            Math.Log( Math.Tan(x) ) * Math.Pow(
                Math.PI, Math.Sin(Math.Pow(x, 2)
                ))
        );
    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys)
        => PulseKeys(pulseInfo with { WaveModulation = _flickerModulation }, keys);

    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys)
        => PulseKeys(pulseInfo with { WaveModulation = _flickerModulation }, keys);
    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKey[] keys)
        => PulseKeys(pulseInfo with { WaveModulation = _flickerModulation }, keys);

    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys)
        => PulseKeys(pulseInfo with { WaveModulation = _flickerModulation }, keys);

    public EffectReceipt FlickerZones(PulseInfo pulseInfo, KeyboardZones zones)
        => PulseZones(pulseInfo with { WaveModulation = _flickerModulation }, zones);
}
