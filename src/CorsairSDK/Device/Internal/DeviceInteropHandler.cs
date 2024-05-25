using System.Reflection;
using Corsair.Tracing;

namespace Corsair.Device.Internal;

using Bindings;
using Contracts;
using Devices;
using OneOf;

internal unsafe class DeviceInteropHandler : IDeviceInterop
{

    /// <summary>
    /// Loads the x86 dll if the current runtime is x86 or Loads the x64 dll if the current tuntime is x64
    /// </summary>
    static DeviceInteropHandler() => NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), SDKResolver.CorsairSDKResolver);

    public CorsairDevice[] GetDevices(DeviceType deviceFilter = DeviceType.All)
    {
        var filter = new CorsairDeviceFilter { deviceTypeMask = (int)deviceFilter };

        var buffer = stackalloc CorsairDeviceInfo[(int)Interop.CORSAIR_DEVICE_COUNT_MAX];
        var count = default(int);

        var result = Interop.GetDevices(&filter, (int)Interop.CORSAIR_DEVICE_COUNT_MAX, buffer, &count);

        InteropTracing.Trace(result, filter, count);

        result.ThrowIfNecessary();

        if (count is 0)
            return [];

        var devices = new CorsairDevice[count];

        for (var i = 0; i < count; i++)
            devices[i] = MapFromBinding(buffer[i]);

        return devices;

    }

    private DeviceType FromCorsairType(CorsairDeviceType type) => (DeviceType)type;

    private CorsairDevice MapFromBinding(CorsairDeviceInfo info)
    {
        var id = CorsairMarshal.ToString(info.id);
        var model = CorsairMarshal.ToString(info.model);
        var serial = CorsairMarshal.ToString(info.serial);
        var type = FromCorsairType(info.type);

        var supportedProperties = GetSupportedProperties(id);

        var deviceInfo = new DeviceInformation(
            id,
            model,
            serial,
            type,
            info.channelCount,
            info.ledCount,
            supportedProperties,
            this);

        switch (type)
        {
            case DeviceType.None:
                throw new NullReferenceException();
            case DeviceType.Keyboard:
                return new Keyboard(deviceInfo);
            case DeviceType.Headset:
                return new Headset(deviceInfo);
            case DeviceType.HeadsetStand:
                return new HeadsetStand(deviceInfo);
            case DeviceType.Mouse:
            case DeviceType.Mousemat:
            case DeviceType.FanLedController:
            case DeviceType.LedController:
            case DeviceType.MemoryModule:
            case DeviceType.Cooler:
            case DeviceType.Motherboard:
            case DeviceType.GraphicsCard:
            case DeviceType.Touchbar:
            case DeviceType.All:
            default:
                return new CorsairDevice(deviceInfo);
        }
    }

    public DeviceProperty[] GetSupportedProperties(string deviceId) => ReadDeviceProperty(deviceId, DeviceProperty.PropertyArray).AsT5.Select(x => (DeviceProperty)x).ToArray();

    public OneOf<bool, int, double, string, bool[], int[], double[], string[]> ReadDeviceProperty(string deviceId, DeviceProperty propertyId)
    {
        var property = default(CorsairProperty);

        var result = Interop.ReadDeviceProperty(CorsairMarshal.ToPointer(deviceId), (CorsairDevicePropertyId)propertyId,
            0, &property);

        InteropTracing.Trace(result, deviceId, propertyId, property);

        result.ThrowIfNecessary();

        try
        {
            switch (property.type)
            {
                case CorsairDataType.CT_Boolean:
                    return property.value.boolean;
                case CorsairDataType.CT_Int32:
                    return property.value.int32;
                case CorsairDataType.CT_Float64:
                    return property.value.float64;
                case CorsairDataType.CT_String:
                    return CorsairMarshal.ToString(property.value.@string);
                case CorsairDataType.CT_Boolean_Array:
                    var boolArray = property.value.boolean_array;
                    return CorsairMarshal.ToArray(boolArray.items, boolArray.count);
                case CorsairDataType.CT_Int32_Array:
                    var intArray = property.value.int32_array;
                    return CorsairMarshal.ToArray(intArray.items, intArray.count);
                case CorsairDataType.CT_Float64_Array:
                    var doubleArray = property.value.float64_array;
                    return CorsairMarshal.ToArray(doubleArray.items, doubleArray.count);
                case CorsairDataType.CT_String_Array:
                    var stringArray = property.value.string_array;
                    return CorsairMarshal.ToArray(stringArray.items, stringArray.count);
                default:
                    throw new ArgumentOutOfRangeException(nameof(property.type));
            }
        }
        finally
        {
            InteropTracing.Trace(Interop.FreeProperty(&property));
        }

    }

    public DevicePropertyInfo GetPropertyInfo(string deviceId, DeviceProperty property)
    {
        var dataType = default(CorsairDataType);
        var flags = default(CorsairPropertyFlag);

        var result = Interop.GetDevicePropertyInfo(CorsairMarshal.ToPointer(deviceId), (CorsairDevicePropertyId)property, 0, &dataType, (uint*)&flags);

        InteropTracing.Trace(result, deviceId, property, dataType, flags);

        return CorsairError.CE_Success == result
            ? new DevicePropertyInfo(dataType, flags)
            : default;
    }
}
