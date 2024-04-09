using System.Diagnostics;

namespace Dawn.CorsairSDK.Rewrite.Exceptions;

public class DeviceNotConnectedException : Exception
{
    public DeviceNotConnectedException() : base("Device is not initialized") => Debugger.Break();
}
