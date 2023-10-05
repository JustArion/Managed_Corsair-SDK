namespace Dawn.Libs.Corsair.SDK;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LowLevel;

public class LedController
{
    internal LedController(ref CorsairDeviceInfo device) => Device = device;

    private readonly CorsairDeviceInfo Device;

    private IDisposable? _deviceControl;
    
    public int LedCount => Device.ledCount;
    

    public unsafe IDisposable RequestControl(CorsairAccessLevel accessLevel)
    {
        _deviceControl?.Dispose();

        fixed (sbyte* deviceId = Device.id)
            Methods.CorsairRequestControl(deviceId, accessLevel).ThrowIfNecessary();

        _deviceControl = LedDisposable.Create(() =>
        {
            fixed (sbyte* deviceId = Device.id)
                Methods.CorsairReleaseControl(deviceId).ThrowIfNecessary();
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
            throw new InvalidOperationException("Method requires an LED Id");

        CorsairError error;
        fixed (sbyte* deviceId = Device.id)
            error = Methods.CorsairSetLedColors(deviceId, 1, &color);

        return error == CorsairError.CE_Success;
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
        fixed (sbyte* deviceId = Device.id)
            error = Methods.CorsairGetLedColors(deviceId, ledColors.Length, device);

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
            fixed (sbyte* deviceId = Device.id)
            error = Methods.CorsairGetLedPositions(deviceId, (int)Methods.CORSAIR_DEVICE_LEDCOUNT_MAX, device, &size);

        if (error != CorsairError.CE_Success)
            return (false, Array.Empty<CorsairLedPosition>());

        Array.Resize(ref ledPositions, size);

        return (true, ledPositions);
    }


    public ValueTask<CorsairError> FlushAsync() => SetLedsColorsFlushBufferAsync();
    public CorsairError Flush(Action<CorsairError> asyncCallback) => SetLedsColorsFlushBuffer(asyncCallback);

    public static async ValueTask<CorsairError> SetLedsColorsFlushBufferAsync()
    {
        var id = Guid.NewGuid();
        var tcs = new TaskCompletionSource<CorsairError>();
        _asyncCallbacks.Add(id, error => tcs.SetResult(error));


        var hGuid = GuidToPointer(id);

        var error = Internal_FlushBufferAsync(hGuid);

        if (error == CorsairError.CE_Success) 
            return await tcs.Task;
        
        _asyncCallbacks.Remove(id);
        return error;

    }

    private static unsafe nint GuidToPointer(Guid id) => (nint)Unsafe.AsPointer(ref id);

    public static unsafe CorsairError SetLedsColorsFlushBuffer(Action<CorsairError>? onCompletion = null)
    {
        if (onCompletion == null)
            return Methods.CorsairSetLedColorsFlushBufferAsync(&SetLedColorsAsyncCallback, null);
        
        var id = Guid.NewGuid();
        _asyncCallbacks.Add(id, onCompletion);

        var hGuid = GuidToPointer(id);

        return Internal_FlushBufferAsync(hGuid);
    }

    private static unsafe CorsairError Internal_FlushBufferAsync(nint context) => Methods.CorsairSetLedColorsFlushBufferAsync(&SetLedColorsAsyncCallback, (void*)context);

    // We set the context as a Guid, the guid contains an Action<CorsairError>, this callback allows us to await until the callback completes.
    // Guid is the indicator for which instance is the caller.
    // Cleanup involes clearing the Guid KeyValuePair from the Dictionary and freeing the pointer allocated from the instance method.
    private static readonly Dictionary<Guid, Action<CorsairError>> _asyncCallbacks = new(); 
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe void SetLedColorsAsyncCallback(void* context, CorsairError error)
    {
        if (context == null)
            return;

        var guid = Unsafe.AsRef<Guid>(context);

        if (!_asyncCallbacks.TryGetValue(guid, out var callback))
            Console.Error.WriteLine($"[!] {guid} has no matching callback.");
        else
        {
            callback(error);
            _asyncCallbacks.Remove(guid);
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

        private volatile bool _disposed;
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            _CleanupAction();
        }
    }
}