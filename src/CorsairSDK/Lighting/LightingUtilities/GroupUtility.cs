using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Corsair.Device;

namespace Corsair.Lighting.LightingUtilities;

public static class GroupUtility
{
    private static ConcurrentDictionary<string, HashSet<KeyboardKey>> _deviceKeys => ZoneUtility._deviceKeys;

    [SuppressMessage("ReSharper", "InvertIf")]
    public static HashSet<KeyboardKey> GetKeysFromGroup(LedGroup group, CorsairDevice device)
    {
        if (!_deviceKeys.TryGetValue(device.Id, out var deviceKeyboardKeys))
        {
            var positions = device.LightingInterop.GetPositionInfo();

            deviceKeyboardKeys = [..positions.Select(x => (KeyboardKey)x.Key)];
            _deviceKeys.TryAdd(device.Id, deviceKeyboardKeys);
        }

        // The Led Group is the first 16 bits of the int that is the KeyboardKey (id)
        // See https://corsairofficial.github.io/cue-sdk/#led-identification
        return deviceKeyboardKeys.Where(x => (int)x >> 16 == (int)group).ToHashSet();
    }
}
