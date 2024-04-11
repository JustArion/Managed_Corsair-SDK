using Dawn.CorsairSDK.Rewrite.Lighting;

namespace Dawn.CorsairSDK.Rewrite;

using System.Diagnostics.CodeAnalysis;

public class ZoneUtility
{

    private static readonly Dictionary<KeyboardZones, Range[]> _zoneMap = new()
    {
        { KeyboardZones.MainZone,
        [
            new((int)KeyboardKeys.ESC, (int)KeyboardKeys.LEFT_SHIFT),
            new((int)KeyboardKeys.Z, (int)KeyboardKeys.RIGHT_CONTROL)
        ] },
        { KeyboardZones.PageKeys, [new((int)KeyboardKeys.PRINT_SCREEN, (int)KeyboardKeys.PAGE_DOWN)] },
        { KeyboardZones.ArrowKeys, [new((int)KeyboardKeys.ARROW_UP, (int)KeyboardKeys.ARROW_RIGHT)] },
        { KeyboardZones.MediaKeys, [new((int)KeyboardKeys.MUTE, (int)KeyboardKeys.MEDIA_NEXT)] },
        { KeyboardZones.NumKeys,
            [
                new((int)KeyboardKeys.NUM_LOCK, (int)KeyboardKeys.NUM_THREE),
                new((int)KeyboardKeys.NUM_ENTER, (int)KeyboardKeys.NUM_PERIOD)
            ]
        },
        {
            KeyboardZones.WASDKeys,
            [
                new((int)KeyboardKeys.W, (int)KeyboardKeys.W),
                new((int)KeyboardKeys.A, (int)KeyboardKeys.D)
            ]
        },
        { KeyboardZones.Logo, [new((int)KeyboardKeys.Logo, (int)KeyboardKeys.LogoBacklight)] },
    };

    // _zoneMap.Select(x => x.Value).Average(x => x.Average(a => (a.End.Value + 1) - a.Start.Value))
    private const int AVERAGE_ZONE_SIZE = 16;

    [SuppressMessage("ReSharper", "InvertIf")]
    public static HashSet<KeyboardKeys> GetKeysFromZone(KeyboardZones zones)
    {
        var keys = new HashSet<KeyboardKeys>(AVERAGE_ZONE_SIZE);

        var eachZone = Enum.GetValues(typeof(KeyboardZones)).Cast<KeyboardZones>();

        foreach (var zone in eachZone)
            if (zones.HasFlag(zone))
            {
                var zoneKeys = KeysForZone(zone);
                foreach (var keyboardKeys in zoneKeys)
                    keys.Add(keyboardKeys);
            }


        return keys;
    }


    private static IEnumerable<KeyboardKeys> KeysForZone(KeyboardZones zone)
        => _zoneMap.TryGetValue(zone, out var ranges)
            ? GetFromRanges(ranges)
            : Enumerable.Empty<KeyboardKeys>();

    private static IEnumerable<KeyboardKeys> GetFromRanges(Range[] ranges)
    {
        foreach (var range in ranges)
            for (var i = range.Start.Value; i < range.End.Value + 1; i++)
                yield return (KeyboardKeys)i;
    }

    public static IEnumerable<int> GetIdsFromKeys(IEnumerable<KeyboardKeys> keys) => keys.Select(x => (int)x);
}
