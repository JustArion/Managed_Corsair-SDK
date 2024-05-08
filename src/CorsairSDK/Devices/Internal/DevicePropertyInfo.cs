using Corsair.Bindings;

namespace Corsair.Device.Internal;

public record struct DevicePropertyInfo(CorsairDataType DataType, CorsairPropertyFlag Flags);
