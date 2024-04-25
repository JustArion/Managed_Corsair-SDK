using Dawn.CorsairSDK.Rewrite.Lighting;

namespace Dawn.CorsairSDK.Rewrite.Device.Internal.Contracts;

internal interface IDeviceController : IDisposable
{
    IDisposable RequestControl(AccessLevel accessLevel);
    void ReleaseControl();
}
