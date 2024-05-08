namespace Corsair.Bindings;

public enum CorsairDeviceType
{
    CDT_Unknown = 0x0000,
    CDT_Keyboard = 0x0001,
    CDT_Mouse = 0x0002,
    CDT_Mousemat = 0x0004,
    CDT_Headset = 0x0008,
    CDT_HeadsetStand = 0x0010,
    CDT_FanLedController = 0x0020,
    CDT_LedController = 0x0040,
    CDT_MemoryModule = 0x0080,
    CDT_Cooler = 0x0100,
    CDT_Motherboard = 0x0200,
    CDT_GraphicsCard = 0x0400,
    CDT_Touchbar = 0x0800,
    CDT_GameController = 0x1000,
    CDT_All = unchecked((int)(0xFFFFFFFF)),
}
