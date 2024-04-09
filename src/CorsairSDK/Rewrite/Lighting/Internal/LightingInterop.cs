// #define TRACK_COLOR
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reactive.Disposables;
using Dawn.CorsairSDK.Bindings;
using Dawn.CorsairSDK.Rewrite.Device.Internal;
using Dawn.CorsairSDK.Rewrite.Exceptions;
using Dawn.CorsairSDK.Rewrite.Lighting.Internal.Contracts;
using Dawn.CorsairSDK.Rewrite.Tracing;

namespace Dawn.CorsairSDK.Rewrite.Lighting.Internal;

internal unsafe class LightingInterop : ILightingInterop
{
    private Device.CorsairDevice? device;

    public void Dispose() => ReleaseControl();

    public IDisposable RequestControl(AccessLevel accessLevel)
    {
        if (device == null)
            throw new DeviceNotConnectedException();


        using (CorsairMarshal.StringToAnsiPointer(device.Id, out var id))
            Track.Interop(Interop.RequestControl(id, ToCorsairAccessLevel(accessLevel)), device.Id, accessLevel).ThrowIfNecessary();

        Debug.WriteLine($"Requested Device Control with access level '{accessLevel}' on device 'Corsair {device.Model}'", "Lighting Interop");

        return Disposable.Create(ReleaseControl);
    }

    private CorsairAccessLevel ToCorsairAccessLevel(AccessLevel accessLevel) => (CorsairAccessLevel)accessLevel;

    public void ReleaseControl()
    {
        if (device == null)
            return;

        Debug.WriteLine("Releasing Device Control", "Lighting Interop");

        using (CorsairMarshal.StringToAnsiPointer(device.Id, out var id))
            Track.Interop(Interop.ReleaseControl(id), param: device.Id);
    }

    public void SetDeviceContext(Device.CorsairDevice deviceContext)
    {
        _positionMap.Clear();
        device = deviceContext;
    }

    private readonly Dictionary<uint, Vector2> _positionMap = new();

    public Vector2 GetPosition(int ledId)
    {
        if (device == null)
            throw new DeviceNotConnectedException();
        var uintId = (uint)ledId;

        if (_positionMap.Count != 0)
            return ProbePositionMap(uintId);

        var buffer = stackalloc CorsairLedPosition[(int)Interop.CORSAIR_DEVICE_LEDCOUNT_MAX];
        var positionsCount = default(int);

        using (CorsairMarshal.StringToAnsiPointer(device.Id, out var id))
            Track.Interop(Interop.GetLedPositions(id, (int)Interop.CORSAIR_DEVICE_LEDCOUNT_MAX, buffer, &positionsCount), param: device.Id).ThrowIfNecessary();


        for (var i = 0; i < positionsCount; i++)
        {
            var current = buffer[i];

            _positionMap.Add(current.id, new Vector2((float)current.cx, (float)current.cy));
        }

        return ProbePositionMap(uintId);
    }

    private Vector2 ProbePositionMap(uint ledId) =>
        _positionMap.TryGetValue(ledId, out var position)
            ? position
            : Vector2.Zero;

    public void SetLedColor(int ledId, Color color)
    {
        if (device == null)
            throw new DeviceNotConnectedException();

        var nColor = ToCorsairColor(color) with { id = (uint)ledId };

        #if TRACK_COLOR
        using (CorsairMarshal.StringToAnsiPointer(device.Id, out var id))
            Track.Interop(Interop.SetLedColors(id, 1, &nColor), device.Id, (R: color.R, G: color.G, B: color.B, A: color.A, Id: ledId)).ThrowIfNecessary();
        #else
        using (CorsairMarshal.StringToAnsiPointer(device.Id, out var id))
            Interop.SetLedColors(id, 1, &nColor).ThrowIfNecessary();
        #endif
    }

    private static CorsairLedColor ToCorsairColor(Color color) =>
        new() {
            r = color.R,
            g = color.G,
            b = color.B,
            a = color.A,
        };

    private static CorsairLedColor _clearColor = ToCorsairColor(Color.Black);
    public void ClearLedColor(int ledId)
    {
        if (device == null)
            return;

        _clearColor.id = (uint)ledId;

        using (CorsairMarshal.StringToAnsiPointer(device.Id, out var id))
            fixed (CorsairLedColor* color = &_clearColor)
                Track.Interop(Interop.SetLedColors(id, 1, color));
    }
}
