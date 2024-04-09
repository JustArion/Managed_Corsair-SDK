using Dawn.CorsairSDK.Bindings;

namespace Dawn.CorsairSDK.Rewrite.Device.Internal;

public record struct DevicePropertyInfo(CorsairDataType DataType, CorsairPropertyFlag Flags);
