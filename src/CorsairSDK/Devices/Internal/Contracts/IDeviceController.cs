using Corsair.Lighting;

namespace Corsair.Device.Internal.Contracts;

internal interface IDeviceController : IDisposable
{
    IDisposable RequestControl(AccessLevel accessLevel);
    void ReleaseControl();
}
