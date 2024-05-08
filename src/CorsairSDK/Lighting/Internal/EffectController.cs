#define TRACE
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Internal;

internal partial class EffectController(KeyboardColorController colorController) : IEffectController
{
    public void Dispose() => _receiptHandler.DisposeAndClear();

    private readonly ReceiptHandler<KeyboardKeys> _receiptHandler = new();

    public void StopEffectsOn(params KeyboardKeys[] keys)
    {
        colorController.ThrowIfDisconnected();
        _receiptHandler.RelinquishAccess(keys);
    }

    public void StopEffectsOn(params KeyboardKey[] keys)
    {
        colorController.ThrowIfDisconnected();
        _receiptHandler.RelinquishAccess(keys.Select(x => x.Key));
    }

    public void StopEffectsOn(KeyboardZones zones)
    {
        colorController.ThrowIfDisconnected();

        var keys = ZoneUtility.GetKeysFromZones(zones);
        _receiptHandler.RelinquishAccess(keys);
    }
}
