namespace Corsair.Lighting;

public static class KeyUtility
{
    public static KeyboardKeys[] From(KeyboardKeys start, KeyboardKeys to) => Enum.GetValues<KeyboardKeys>().Where(k => k >= start && k <= to).ToArray();
}
