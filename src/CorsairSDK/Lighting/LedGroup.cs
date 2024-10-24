using Corsair.Attributes;

namespace Corsair.Lighting;

[DomainImplOf<Bindings.CorsairLedGroup>]
public enum LedGroup
{
    /// <summary>
    /// For Keyboard Leds
    /// </summary>
    Keyboard = 0,

    /// <summary>
    /// For Keyboard Leds on G-Keys
    /// </summary>
    KeyboardGKeys = 1,

    /// <summary>
    /// For Keyboard Lighting Pipe Leds
    /// </summary>
    KeyboardEdge = 2,

    /// <summary>
    /// For vendor specific Keyboard Leds
    /// </summary>
    KeyboardOEM = 3,

    Mouse = 4,
    Mousemat = 5,
    Headset = 6,
    HeadsetStand = 7,
    MemoryModule = 8,
    Motherboard = 9,
    GraphicsCard = 10,

    /// <summary>
    /// For Leds on the first channel of DIY devices and coolers
    /// </summary>
    DIYChannel1 = 11,
    /// <summary>
    /// For Leds on the first channel of DIY devices and coolers
    /// </summary>
    DIYChannel2 = 12,
    /// <summary>
    /// For Leds on the first channel of DIY devices and coolers
    /// </summary>
    DIYChannel3 = 13,

    Touchbar = 14,
    GameController = 15
}
