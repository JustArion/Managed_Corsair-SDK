using System.Diagnostics;

namespace Corsair.Exceptions;

public class DeviceNotConnectedException : CorsairException
{
    public DeviceNotConnectedException() : base("Device is not initialized") => Debugger.Break();
}
