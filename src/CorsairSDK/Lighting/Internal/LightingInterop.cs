// #define TRACK_COLOR
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reactive.Disposables;
using Corsair.Bindings;
using Corsair.Device.Internal;
using Corsair.Exceptions;
using Corsair.Lighting.Contracts;
using Corsair.Tracing;

namespace Corsair.Lighting.Internal;

internal unsafe class LightingInterop : ILightingInterop
{
    private Device.CorsairDevice? device;
    private IDisposable? _internalState;

    public void Dispose() => ReleaseControl();

    private const int MAX_LEDS = (int)Interop.CORSAIR_DEVICE_LEDCOUNT_MAX;

    public Dictionary<int, LedInfo> GetPositionInfo()
    {
        if (device == null)
            throw new DeviceNotConnectedException();

        var retVal = new Dictionary<int, LedInfo>();

        var positionBuffer = stackalloc CorsairLedPosition[MAX_LEDS];
        var count = default(int);


        var deviceId = CorsairMarshal.ToPointer(device.Id);
        var result =
            Interop.GetLedPositions(deviceId, MAX_LEDS, positionBuffer, &count);

        InteropTracing.DebugTrace(result, param: device.Id);

        result.ThrowIfNecessary();

        var colorBuffer = stackalloc CorsairLedColor[count];

        for (var i = 0; i < count; i++)
            colorBuffer[i].id = positionBuffer[i].id;


        result = Interop.GetLedColors(deviceId, count, colorBuffer);

        InteropTracing.DebugTrace(result, param: device.Id);

        result.ThrowIfNecessary();

        for (var i = 0; i < count; i++)
        {
            var led = positionBuffer[i];
            var color = colorBuffer[i];
            var info = new LedInfo(Color.FromArgb(color.a, color.r, color.g, color.b), new Vector2((float)led.cx, (float)led.cy));
            retVal.Add((int)led.id, info);
        }

        return retVal;
    }

    public IDisposable RequestControl(AccessLevel accessLevel)
    {
        if (device == null)
            throw new DeviceNotConnectedException();

        if (_internalState != null)
            return _internalState;

        var result = Interop.RequestControl(CorsairMarshal.ToPointer(device.Id), ToCorsairAccessLevel(accessLevel));

        InteropTracing.DebugTrace(result, device.Id, accessLevel);

        result.ThrowIfNecessary();

        Debug.WriteLine($"Requested Device Control with access level '{accessLevel}' on device 'Corsair {device.Model}'", "Lighting Interop");

        _internalState = Disposable.Create(ReleaseControl);
        return _internalState;
    }

    private CorsairAccessLevel ToCorsairAccessLevel(AccessLevel accessLevel) => (CorsairAccessLevel)accessLevel;

    public void ReleaseControl()
    {
        if (device == null)
            return;

        Debug.WriteLine("Releasing Device Control", "Lighting Interop");

        var result = Interop.ReleaseControl(CorsairMarshal.ToPointer(device.Id));

        InteropTracing.DebugTrace(result, param: device.Id);

        if (result == CorsairError.CE_Success)
            _internalState = null;
    }

    public void SetDeviceContext(Device.CorsairDevice deviceContext)
    {
        if (deviceContext == null)
            throw new NullReferenceException(nameof(deviceContext));

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

        var result = Interop.GetLedPositions(CorsairMarshal.ToPointer(device.Id),
            (int)Interop.CORSAIR_DEVICE_LEDCOUNT_MAX, buffer, &positionsCount);

        InteropTracing.DebugTrace(result,  param: device.Id);

        result.ThrowIfNecessary();

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

        var result = Interop.SetLedColors(CorsairMarshal.ToPointer(device.Id), 1, &nColor);

        InteropTracing.Interop(result, device.Id, (color.R, color.G, color.B, color.A, Id: ledId));

        result.ThrowIfNecessary();
        #else
        Interop.SetLedColors(CorsairMarshal.ToPointer(device.Id), 1, &nColor).ThrowIfNecessary();
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

        fixed (CorsairLedColor* color = &_clearColor)
            InteropTracing.DebugTrace(
                Interop.SetLedColors(CorsairMarshal.ToPointer(device.Id), 1, color)
                );
    }
}
