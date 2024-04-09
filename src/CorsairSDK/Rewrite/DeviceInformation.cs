namespace Dawn.CorsairSDK.Rewrite;

using Device;
using Device.Internal.Contracts;

internal record struct DeviceInformation(
    string Id,
    string Model,
    string Serial,
    DeviceType Type,
    int ChannelCount,
    int LedCount,
    DeviceProperty[] SupportedProperties,
    IDeviceInterop InteropLayer);
