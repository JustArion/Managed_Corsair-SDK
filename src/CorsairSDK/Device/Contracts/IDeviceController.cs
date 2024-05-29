using Corsair.Lighting;

namespace Corsair.Device.Contracts;

public interface IDeviceController : IDisposable
{
    IDisposable RequestControl(AccessLevel accessLevel);
    void ReleaseControl();
}
