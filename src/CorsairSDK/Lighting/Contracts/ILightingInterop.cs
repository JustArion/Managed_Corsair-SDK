namespace Corsair.Lighting.Contracts;

using System.Drawing;
using System.Numerics;
using Device;
using Device.Contracts;

public interface ILightingInterop : IDeviceController
{
    /// <exception cref="System.NullReferenceException">The device is null</exception>
    void SetDeviceContext(CorsairDevice deviceContext);

    /// <returns>A collection of all leds, their ids, positions and colors</returns>
    /// <exception cref="T:Corsair.Exceptions.DeviceNotConnectedException">The device is not connected, the operation could not be completed</exception>
    Dictionary<int, LedInfo> GetPositionInfo();

    /// <returns>The position of the led with the given id</returns>
    /// <exception cref="T:Corsair.Exceptions.DeviceNotConnectedException">The device is not connected, the operation could not be completed</exception>
    /// <exception cref="T:Corsair.Exceptions.CorsairException">An unexpected event happened, the device may have gotten disconnected</exception>
    Vector2 GetPosition(int ledId);

    // For the moment, this is all we should handle, then expand on that later.
    // We can expand later into async (on many keys) then handle when neccessary without the user knowing.
    // We the user doesn't need to know that async is happening, they only need to know that their keys arn't working fast.
    // So we don't need to burden the end-user with async.
    /// <exception cref="T:Corsair.Exceptions.DeviceNotConnectedException">The device is not connected, the operation could not be completed</exception>
    /// <exception cref="T:Corsair.Exceptions.CorsairException">An unexpected event happened, the device may have gotten disconnected</exception>
    void SetLedColor(int ledId, Color color);

    void ClearLedColor(int ledId);

}
