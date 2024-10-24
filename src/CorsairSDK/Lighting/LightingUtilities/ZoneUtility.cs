﻿using System.Collections.Concurrent;
using Corsair.Device;
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
            new((int)KeyboardKey.Esc, (int)KeyboardKey.LeftShift),
            new((int)KeyboardKey.Z, (int)KeyboardKey.RightControl),
            new Range(129, 129) // FN
        ] },
        { KeyboardZones.PageKeys, [new((int)KeyboardKey.PrintScreen, (int)KeyboardKey.PageDown)] },
        { KeyboardZones.ArrowKeys, [new((int)KeyboardKey.ArrowUp, (int)KeyboardKey.ArrowRight)] },
        { KeyboardZones.MediaKeys, [new((int)KeyboardKey.Mute, (int)KeyboardKey.MediaNext)] },
        { KeyboardZones.NumKeys,
            [
                new((int)KeyboardKey.NumLock, (int)KeyboardKey.NumThree),
                new((int)KeyboardKey.NumEnter, (int)KeyboardKey.NumPeriod)
            ]
        },
        {
            KeyboardZones.WASDKeys,
            [
                new((int)KeyboardKey.W, (int)KeyboardKey.W),
                new((int)KeyboardKey.A, (int)KeyboardKey.D)
            ]
        },
        { KeyboardZones.Logo, [new((int)KeyboardKey.OEM_Led1, (int)KeyboardKey.OEM_Led5)] },
        { KeyboardZones.GKEYS, [new ((int)KeyboardKey.G_Key1, (int)KeyboardKey.G_Key6)] },
    };

    // _zoneMap.Select(x => x.Value).Average(x => x.Average(a => (a.End.Value + 1) - a.Start.Value))
    private const int AVERAGE_ZONE_SIZE = 16;

    internal static readonly ConcurrentDictionary<string, HashSet<KeyboardKey>> _deviceKeys = new();

    public static HashSet<KeyboardKey> GetKeysFromZones(KeyboardZones zones, Keyboard forDevice)
    {
        var keys = new HashSet<KeyboardKey>(forDevice.LedCount / 5); // Rough estimate

        if (!_deviceKeys.TryGetValue(forDevice.Id, out var deviceKeyboardKeys))
        {
            var positions = forDevice.LightingInterop.GetPositionInfo();

            deviceKeyboardKeys = [..positions.Select(x => (KeyboardKey)x.Key)];
            _deviceKeys.TryAdd(forDevice.Id, deviceKeyboardKeys);
        }

        if (zones.HasFlag(KeyboardZones.AllZones))
            return deviceKeyboardKeys;

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