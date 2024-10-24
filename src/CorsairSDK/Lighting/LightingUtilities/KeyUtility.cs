﻿namespace Corsair.Lighting;

public static class KeyUtility
{
    public static KeyboardKey[] From(KeyboardKey start, KeyboardKey to) => Enum.GetValues<KeyboardKey>().Where(k => k >= start && k <= to).ToArray();
}