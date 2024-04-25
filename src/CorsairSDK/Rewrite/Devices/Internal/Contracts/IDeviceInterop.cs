namespace Dawn.CorsairSDK.Rewrite.Device.Internal.Contracts;

using OneOf;

internal interface IDeviceInterop
{
    CorsairDevice[] GetDevices(DeviceType deviceFilter = DeviceType.All);

    DeviceProperty[] GetSupportedProperties(string deviceId);

    OneOf<bool, int, double, string, bool[], int[], double[], string[]> ReadDeviceProperty(string deviceId, DeviceProperty propertyId);

    DevicePropertyInfo GetPropertyInfo(string deviceId, DeviceProperty property);
}
