using Corsair.Device.Devices;
using Corsair.Lighting;

namespace Corsair;

using System.Diagnostics.CodeAnalysis;

public class ZoneUtility
{

    private static readonly Dictionary<KeyboardZones, Range[]> _zoneMap = new()
    {
        { KeyboardZones.MainZone,
        [
            new((int)KeyboardKey.ESC, (int)KeyboardKey.LEFT_SHIFT),
            new((int)KeyboardKey.Z, (int)KeyboardKey.RIGHT_CONTROL),
            new Range(129, 129) // FN
        ] },
        { KeyboardZones.PageKeys, [new((int)KeyboardKey.PRINT_SCREEN, (int)KeyboardKey.PAGE_DOWN)] },
        { KeyboardZones.ArrowKeys, [new((int)KeyboardKey.ARROW_UP, (int)KeyboardKey.ARROW_RIGHT)] },
        { KeyboardZones.MediaKeys, [new((int)KeyboardKey.MUTE, (int)KeyboardKey.MEDIA_NEXT)] },
        { KeyboardZones.NumKeys,
            [
                new((int)KeyboardKey.NUM_LOCK, (int)KeyboardKey.NUM_THREE),
                new((int)KeyboardKey.NUM_ENTER, (int)KeyboardKey.NUM_PERIOD)
            ]
        },
        {
            KeyboardZones.WASDKeys,
            [
                new((int)KeyboardKey.W, (int)KeyboardKey.W),
                new((int)KeyboardKey.A, (int)KeyboardKey.D)
            ]
        },
        { KeyboardZones.Logo, [new((int)KeyboardKey.SpecialLight1, (int)KeyboardKey.SpecialLight5)] },
    };

    // _zoneMap.Select(x => x.Value).Average(x => x.Average(a => (a.End.Value + 1) - a.Start.Value))
    private const int AVERAGE_ZONE_SIZE = 16;

    [SuppressMessage("ReSharper", "InvertIf")]
    internal static HashSet<KeyboardKey> GetKeysFromZones(KeyboardZones zones)
    {
        var keys = new HashSet<KeyboardKey>(AVERAGE_ZONE_SIZE);

        var eachZone = Enum.GetValues<KeyboardZones>();

        foreach (var zone in eachZone)
            if (zones.HasFlag(zone))
            {
                var zoneKeys = KeysForZone(zone);
                foreach (var keyboardKeys in zoneKeys)
                    keys.Add(keyboardKeys);
            }


        return keys;
    }

    private static readonly Dictionary<string, HashSet<KeyboardKey>> _deviceKeys = new();

    public static HashSet<KeyboardKey> GetKeysFromZones(KeyboardZones zones, Keyboard forDevice)
    {
        var keys = new HashSet<KeyboardKey>(forDevice.LedCount);

        if (!_deviceKeys.TryGetValue(forDevice.Id, out var deviceKeyboardKeys))
        {
            var positions = forDevice.LightingInterop.GetPositionInfo();

            deviceKeyboardKeys = [..positions.Select(x => (KeyboardKey)x.Key)];
            _deviceKeys[forDevice.Id] = deviceKeyboardKeys;
        }

        foreach (var zone in Enum.GetValues<KeyboardZones>())
            if (zones.HasFlag(zone))
                foreach (var key in KeysForZone(zone))
                    if (deviceKeyboardKeys.Contains(key))
                        keys.Add(key);

        return keys;
    }

    private static IEnumerable<KeyboardKey> KeysForZone(KeyboardZones zone)
        => _zoneMap.TryGetValue(zone, out var ranges)
            ? GetFromRanges(ranges)
            : [];

    private static IEnumerable<KeyboardKey> GetFromRanges(Range[] ranges)
    {
        foreach (var range in ranges)
            for (var i = range.Start.Value; i < range.End.Value + 1; i++)
                yield return (KeyboardKey)i;
    }

    public static IEnumerable<int> GetIdsFromKeys(IEnumerable<KeyboardKey> keys) => keys.Select(x => (int)x);
}
