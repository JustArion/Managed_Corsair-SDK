using System.Diagnostics;

namespace Corsair.Exceptions;

public class DeviceNotConnectedException : Exception
{
    public DeviceNotConnectedException() : base("Device is not initialized") => Debugger.Break();
}
