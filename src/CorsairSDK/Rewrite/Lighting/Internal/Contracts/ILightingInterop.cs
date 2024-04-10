﻿namespace Dawn.CorsairSDK.Rewrite.Lighting.Internal.Contracts;

using System.Drawing;
using System.Numerics;
using Device;
using Device.Internal.Contracts;

internal interface ILightingInterop : IDeviceController
{
    void SetDeviceContext(CorsairDevice deviceContext);

    Vector2 GetPosition(int ledId);
    
    // For the moment, this is all we should handle, then expand on that later.
    // We can expand later into async (on many keys) then handle when neccessary without the user knowing.
    // We the user doesn't need to know that async is happening, they only need to know that their keys arn't working fast.
    // So we don't need to burden the end-user with async.
    void SetLedColor(int ledId, Color color);
    void ClearLedColor(int ledId);
    
}