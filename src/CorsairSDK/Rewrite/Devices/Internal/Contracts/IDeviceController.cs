using Dawn.CorsairSDK.Rewrite.Lighting;

namespace Dawn.CorsairSDK.Rewrite.Device.Internal.Contracts;

using Bindings;

internal interface IDeviceController : IDisposable
{
    IDisposable RequestControl(AccessLevel accessLevel);
    void ReleaseControl();
}
