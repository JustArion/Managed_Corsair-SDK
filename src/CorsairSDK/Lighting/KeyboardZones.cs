namespace Corsair;

[Flags]
public enum KeyboardZones
{
    None = 0,

    /// <summary>
    /// The Esc -> Right Ctrl Zone (Same as Column1)
    /// </summary>
    MainZone = 1 << 0,

    /// <summary>
    /// Print Screen Zone
    /// </summary>
    PageKeys = 1 << 1,

    /// <summary>
    /// Numpad Keys
    /// </summary>
    NumKeys = 1 << 2,

    /// <summary>
    /// Arrow Keys Zone
    /// </summary>
    ArrowKeys = 1 << 3,

    /// <summary>
    /// Mute, Stop, Prev, Pause / Play, Next
    /// </summary>
    MediaKeys = 1 << 4,

    WASDKeys = 1 << 5,

    Logo = 1 << 6,

    AllZones = MainZone | PageKeys | NumKeys | ArrowKeys | MediaKeys | WASDKeys | Logo
}
