namespace Corsair.Device;

public enum DeviceType
{
    None = 0,
    Keyboard = 1 << 0,
    Mouse = 1 << 1,
    Mousemat = 1 << 2,
    Headset = 1 <<  3,
    HeadsetStand = 1 << 4,
    FanLedController = 1 << 5,
    LedController = 1 << 6,
    MemoryModule = 1 << 7,
    Cooler = 1 << 8,
    Motherboard = 1 << 9,
    GraphicsCard = 1 << 10,
    Touchbar = 1 << 11,
    All = -0x1
}
