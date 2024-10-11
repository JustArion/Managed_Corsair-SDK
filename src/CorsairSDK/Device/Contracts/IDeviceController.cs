using Corsair.Lighting;

namespace Corsair.Device.Contracts;

public interface IDeviceController : IDisposable
{
    /// <summary>
    /// Request exclusive / shared control over the device
    /// </summary>
    /// <exception cref="T:Corsair.Exceptions.DeviceNotConnectedException">The device is not connected, the operation could not be completed</exception>
    IDisposable RequestControl(AccessLevel accessLevel);
    void ReleaseControl();
}
