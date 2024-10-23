#define TRACE
using Corsair.Device.Devices;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Internal;

internal partial class EffectController(KeyboardColorController colorController) : IEffectController
{
    private Keyboard _device => colorController.Device;

    public void Dispose() => _receiptHandler.DisposeAndClear();

    private readonly ReceiptHandler<KeyboardKey> _receiptHandler = new();

    public void StopEffectsOn(params KeyboardKey[] keys)
    {
        colorController.ThrowIfDisconnected();
        _receiptHandler.RelinquishAccess(keys);
    }

    public void StopEffectsOn(params KeyboardKeyState[] keys)
    {
        colorController.ThrowIfDisconnected();
        _receiptHandler.RelinquishAccess(keys.Select(x => x.Key));
    }

    public void StopEffectsOn(KeyboardZones zones)
    {
        colorController.ThrowIfDisconnected();

        var keys = ZoneUtility.GetKeysFromZones(zones, _device);
        _receiptHandler.RelinquishAccess(keys);
    }
}
