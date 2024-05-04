using Dawn.CorsairSDK.Rewrite.Lighting.Contracts;

namespace Dawn.CorsairSDK.Rewrite.Lighting.Internal;

// public record FlashInfo(Color Color, TimeSpan FlashInterval, TimeSpan FlashDuration);
internal partial class EffectController
{
    public EffectReceipt FlashKeys(FlashInfo pulseInfo, IEnumerable<KeyboardKeys> keys)
    {
        colorController.ThrowIfDisconnected();

        throw new NotImplementedException();
    }
    public EffectReceipt FlashKeys(FlashInfo flashInfo, params KeyboardKeys[] keys)
        => FlashKeys(flashInfo, (IEnumerable<KeyboardKeys>)keys);

    public EffectReceipt FlashKeys(FlashInfo pulseInfo, params KeyboardKey[] keys)
        => FlashKeys(pulseInfo, keys.Select(x => x.Key));

    public EffectReceipt FlashKeys(FlashInfo pulseInfo, IEnumerable<KeyboardKey> keys)
        => FlashKeys(pulseInfo, keys.Select(x => x.Key));

    public EffectReceipt FlashZones(FlashInfo pulseInfo, KeyboardZones zones)
    {
        var keys = ZoneUtility.GetKeysFromZones(zones);

        return FlashKeys(pulseInfo, keys);
    }
}
