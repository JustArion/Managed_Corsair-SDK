namespace Corsair.Device;

public enum DeviceProperty
{
    /// Dummy value
    Invalid = 0,

    /// Array of CorsairDevicePropertyId members supported by device
    PropertyArray = 1,

    /// Indicates Mic state (On or Off); used for headset, headset stand
    MicEnabled = 2,

    /// Indicates Surround Sound state (On or Off); used for headset, headset stand
    SurroundSoundEnabled = 3,

    /// Indicates Sidetone state (On or Off); used for headset (where applicable)
    SidetoneEnabled = 4,

    /// The number of active equalizer preset (integer, 1 - 5); used for headset, headset stand
    EqualizerPreset = 5,

    /// Keyboard physical layout (see CorsairPhysicalLayout for valid values); used for keyboard
    PhysicalLayout = 6,

    /// Keyboard logical layout (see CorsairLogicalLayout for valid values); used for keyboard
    LogicalLayout = 7,

    /// Array of programmable G, M or S keys on device
    MacroKeyArray = 8,

    /// Battery level (0 - 100); used for wireless devices
    BatteryLevel = 9,

    /// Total number of LEDs connected to the channel
    ChannelLedCount = 10,

    /// Number of LED-devices (fans, strips, etc.) connected to the channel which is controlled by the DIY device
    ChannelDeviceCount = 11,

    /// Array of integers, each element describes the number of LEDs controlled by the channel device
    ChannelDeviceLedCountArray = 12,

    /// Array of CorsairChannelDeviceType members, each element describes the type of the channel device
    ChannelDeviceTypeArray = 13,
}
