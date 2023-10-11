namespace Dawn.CorsairSDK;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Extensions;
using LowLevel;

public struct LedController
{
    internal LedController(in CorsairDeviceInfo device)
    {
        LedCount = device.ledCount;

        _deviceInfo = device;
    }

    private readonly CorsairDeviceInfo _deviceInfo;

    private IDisposable? _deviceControl;
    
    public readonly int LedCount;
    

    public unsafe IDisposable RequestControl(CorsairAccessLevel accessLevel)
    {
        _deviceControl?.Dispose();

        fixed (sbyte* id = _deviceInfo.id)
            Methods.CorsairRequestControl(id, accessLevel).ThrowIfNecessary();

        var idAsString = _deviceInfo.GetID();
        _deviceControl = LedDisposable.Create(() =>
        {
            var hId = Marshal.StringToHGlobalAnsi(idAsString);
            try
            {
                Methods.CorsairReleaseControl((sbyte*)hId).ThrowIfNecessary();
            }
            finally
            {
                Marshal.FreeHGlobal(hId);
            }
        });
        
        return _deviceControl;
    }

    public void ReleaseControl() => _deviceControl?.Dispose();


    public bool TrySetLedColor(uint id, (byte R, byte G, byte B, byte A) colors) => TrySetLedColor(new CorsairLedColor { id = id, r = colors.R, g = colors.G, b = colors.B, a = colors.A });
    public bool TrySetLedColor(CorsairLedPosition position, (byte R, byte G, byte B, byte A) colors) => TrySetLedColor(new CorsairLedColor { id = position.id, r = colors.R, g = colors.G, b = colors.B, a = colors.A });
    public bool TrySetLedColor(LedInformation information)
    {
        var color = information.Color;
        if (color.id == default)
            color = color with { id = information.Position.id };

        return TrySetLedColor(color);
    }
    public bool TrySetLedColor(uint id, CorsairLedColor color)
    {
        if (color.id == default)
            color = color with { id = id };

        return TrySetLedColor(color);
    }
    public bool TrySetLedColor(CorsairLedPosition position, CorsairLedColor color)
    {
        if (color.id == default)
            color = color with { id = position.id };

        return TrySetLedColor(color);
    }
    
    public unsafe bool TrySetLedColor(CorsairLedColor color)
    {
        if (color.id == default)
            return false;

        CorsairError error;
        fixed (sbyte* deviceid = _deviceInfo.id)
            error = Methods.CorsairSetLedColors(deviceid, 1, &color);

        return error == CorsairError.CE_Success;
    }

    public async Task Gradient(CorsairLedColor[] from, CorsairLedColor[] to, TimeSpan animationDuration)
    {
        if (from.Length != to.Length)
            throw new InvalidOperationException("The animation targets should have the same length");

        if (from.Any(x => x.id == default) || to.Any(x => x.id == default))
            throw new InvalidOperationException("All Leds should have an Id associated with it");

        var startTime = DateTimeOffset.Now;
        
        while ((DateTimeOffset.Now - startTime) < animationDuration)
        {
            var progress = (DateTimeOffset.Now - startTime).TotalMilliseconds / animationDuration.TotalMilliseconds;
            for (var i = 0; i < from.Length; i++)
            {
                var lerpedColor = InterpolateColor(from[i], to[i], progress);

                await TrySetLedColorsAsync(lerpedColor);
            }
        }
        
        for (var i = 0; i < from.Length; i++) 
            await TrySetLedColorsAsync(to);
    }
    private static CorsairLedColor InterpolateColor(CorsairLedColor from, CorsairLedColor to, double progress)
    {
        var lerpedR = (byte)(from.r + (to.r - from.r) * progress);
        var lerpedG = (byte)(from.g + (to.g - from.g) * progress);
        var lerpedB = (byte)(from.b + (to.b - from.b) * progress);
        var lerpedA = (byte)(from.a + (to.a - from.a) * progress);

        return new CorsairLedColor
        {
            id = from.id,
            r = lerpedR,
            g = lerpedG,
            b = lerpedB,
            a = lerpedA
        };
    }
    
    public async Task<bool> TrySetLedColorsAsync((uint id, (byte R, byte G, byte B, byte A) RGBA)[] colors)
    {
        var ledColors = new CorsairLedColor[colors.Length];
        for (var i = 0; i < colors.Length; i++)
        {
            var x = colors[i];
            ledColors[i] = new CorsairLedColor
            {
                id = x.id, 
                r = x.RGBA.R, 
                g = x.RGBA.G, 
                b = x.RGBA.B, 
                a = x.RGBA.A
            };
        }

        return await TrySetLedColorsAsync(ledColors);
    }

    public async Task<bool> TrySetLedColorsAsync(CorsairLedPosition[] position, (byte R, byte G, byte B, byte A)[] colors)
    {
        if (position.Length != colors.Length)
            return false;
        
        
        var ledColors = new CorsairLedColor[colors.Length];
        for (var i = 0; i < colors.Length; i++)
        {
            var x = colors[i];
            ledColors[i] = new CorsairLedColor
            {
                id = position[i].id, 
                r = x.R, 
                g = x.G, 
                b = x.B, 
                a = x.A
            };
        }
        return await TrySetLedColorsAsync(ledColors);
    }

    public async Task<bool> TrySetLedColorsAsync(params LedInformation[] information)
    {
        var colors = new CorsairLedColor[information.Length];
        for (var i = 0; i < information.Length; i++)
        {
            var color = information[i].Color;
            if (color.id == default)
                colors[i] = color with { id = information[i].Position.id };
            else colors[i] = color;
        }
        
        

        return await TrySetLedColorsAsync(colors);
    }
    public async Task<bool> TrySetLedColorsAsync(params (uint Id, CorsairLedColor Color)[] Colors)
    {
        var colors = new CorsairLedColor[Colors.Length];
        
        for (var i = 0; i < Colors.Length; i++)
        {
            var color = Colors[i].Color;
            var id = Colors[i].Id;

            if (color.id == default)
                colors[i] = color with { id = id };
            else colors[i] = color;
        }

        return await TrySetLedColorsAsync(colors);
    }
    public async Task<bool> TrySetLedColorsAsync(CorsairLedPosition[] positions, CorsairLedColor[] colors)
    {
        if (positions.Length != colors.Length)
            return false;
        
        for (var i = 0; i < colors.Length; i++)
        {
            var current = colors[i];

            // We add the id to the struct by creating a new struct with the same rgba info but with the new id.
            if (current.id == default)
                colors[i] = current with { id = positions[i].id };
            else colors[i] = current;
        }

        return await TrySetLedColorsAsync(colors);
    }
    public async Task<bool> TrySetLedColorsAsync(params CorsairLedColor[] colors)
    {
        if (colors.Length == 1)
            TrySetLedColor(colors[0]);

        if (colors.Any(x => x.id == default))
            return false; // Id should be pre-set.

        Internal_SetLedColorBuffer(colors);

        var error = await FlushAsync();

        return error == CorsairError.CE_Success;
    }

    private unsafe void Internal_SetLedColorBuffer(CorsairLedColor[] colors)
    {
        fixed (CorsairLedColor* firstElement = &colors[0])
        fixed (sbyte* deviceid = _deviceInfo.id)
            Methods.CorsairSetLedColorsBuffer(deviceid, colors.Length, firstElement);
    }
    
    public (bool Success, LedInformation[] LedInformation) TryGetLedInformation()
    {
        var (positionsSuccess, positions) = TryGetLedPositions();

        if (!positionsSuccess)
            return (false, Array.Empty<LedInformation>());

        var (colorsSuccess, colors) = TryGetLedColors(positions);

        if (!colorsSuccess)
            return (false, Array.Empty<LedInformation>());


        var ledInformation = new LedInformation[positions.Length];
        for (var i = 0; i < positions.Length; i++) 
            ledInformation[i] = new LedInformation(positions[i], colors[i]);

        return (true, ledInformation);
    }

    public unsafe (bool Success, CorsairLedColor[] LedColors) TryGetLedColors(params CorsairLedPosition[] ledPositions)
    {
        var ledColors = new CorsairLedColor[ledPositions.Length];
        
        for (var i = 0; i < ledPositions.Length; i++) 
            ledColors[i] = new() { id = ledPositions[i].id };

        CorsairError error;
        fixed (CorsairLedColor* device = &ledColors[0])
        fixed (sbyte* deviceid = _deviceInfo.id)
            error = Methods.CorsairGetLedColors(deviceid, ledColors.Length, device);

        return error != CorsairError.CE_Success 
            ? (false, Array.Empty<CorsairLedColor>()) 
            : (true, ledColors);
    }
    
    public unsafe (bool Success, CorsairLedPosition[] LedPositions) TryGetLedPositions()
    {
        var ledPositions = new CorsairLedPosition[Methods.CORSAIR_DEVICE_LEDCOUNT_MAX];
        var size = default(int);

        CorsairError error;
        fixed (CorsairLedPosition* device = &ledPositions[0])
        fixed (sbyte* deviceid = _deviceInfo.id)
            error = Methods.CorsairGetLedPositions(deviceid, (int)Methods.CORSAIR_DEVICE_LEDCOUNT_MAX, device, &size);

        if (error != CorsairError.CE_Success)
            return (false, Array.Empty<CorsairLedPosition>());

        Array.Resize(ref ledPositions, size);

        return (true, ledPositions);
    }


    public static readonly (byte R, byte G, byte B, byte A) LedOffColor = (0, 0, 0, 255);

    public static CorsairLedColor ToOffLedColor(uint id) => new()
        { id = id, r = LedOffColor.R, g = LedOffColor.G, b = LedOffColor.B, a = LedOffColor.A };

    public static CorsairLedColor[] ToOffLedColors(IEnumerable<CorsairLedColor> colors) => colors.Select(x => ToOffLedColor(x.id)).ToArray();


    public static ValueTask<CorsairError> FlushAsync() => SetLedsColorsFlushBufferAsync();
    public static CorsairError Flush(Action<CorsairError> asyncCallback) => SetLedsColorsFlushBuffer(asyncCallback);

    public static async ValueTask<CorsairError> SetLedsColorsFlushBufferAsync()
    {
        var id = Guid.NewGuid();
        var tcs = new TaskCompletionSource<CorsairError>();
        
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(3));
        cts.Token.Register(() => tcs.TrySetException(new TimeoutException()));
        
        _asyncCallbacks.Add(id, error => tcs.SetResult(error));


        var error = Internal_FlushBufferAsync(id);

        if (error == CorsairError.CE_Success) 
            return await tcs.Task;
        
        _asyncCallbacks.Remove(id);
        return error;

    }

    public static unsafe CorsairError SetLedsColorsFlushBuffer(Action<CorsairError>? onCompletion = null)
    {
        if (onCompletion == null)
            return Methods.CorsairSetLedColorsFlushBufferAsync(&SetLedColorsAsyncCallback, null);
        
        var id = Guid.NewGuid();
        _asyncCallbacks.Add(id, onCompletion);

        return Internal_FlushBufferAsync(id);
    }

    private static unsafe CorsairError Internal_FlushBufferAsync(Guid contextId)
    {
        var hContext = Marshal.AllocHGlobal(sizeof(Guid));
        Marshal.StructureToPtr(contextId, hContext, true);
        
        return Methods.CorsairSetLedColorsFlushBufferAsync(&SetLedColorsAsyncCallback, (void*)hContext);
    }

    // We set the context as a Guid, the guid contains an Action<CorsairError>, this callback allows us to await until the callback completes.
    // Guid is the indicator for which instance is the caller.
    // Cleanup involes clearing the Guid KeyValuePair from the Dictionary and freeing the pointer allocated from the instance method.
    private static readonly Dictionary<Guid, Action<CorsairError>> _asyncCallbacks = new(); 
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe void SetLedColorsAsyncCallback(void* context, CorsairError error)
    {
        if (context == null)
            return;

        var hContext = (nint)context;

        var guid = Marshal.PtrToStructure<Guid>(hContext);
        
        try
        {
            if (!_asyncCallbacks.TryGetValue(guid, out var callback))
                Console.Error.WriteLine($"[!] {guid} has no matching callback.");
            else
            {
                callback(error);
                _asyncCallbacks.Remove(guid);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(hContext);
        }
    }
    
    private class LedDisposable : IDisposable
    {
        private LedDisposable(Action cleanupAction) => _CleanupAction = cleanupAction;

        public static IDisposable Create(Action cleanup)
        {
            if (cleanup == null) throw new ArgumentNullException(nameof(cleanup));
            
            return new LedDisposable(cleanup);
        }

        private readonly Action _CleanupAction;

        private int _cleanupPerformed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _cleanupPerformed, 1) == 0) 
                _CleanupAction();
        }
    }
}