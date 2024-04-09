namespace Dawn.CorsairSDK;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LowLevel;

public static unsafe class CorsairSDK
{

    public static SDKConfigurationPreferences Preferences { get; private set; } = new();
    public static bool Connect() => EnsureConnected();

    public static void Disconnect()
    {
        if (!_isConnected)
            return;
        
        Methods.CorsairDisconnect();
    }

    /// <summary>
    /// checks versions of SDK client, server and host (iCUE) to understand which of SDK functions can be used with this version of iCUE. If there is no active session or client is not connected to the server, then only client version will be filled.
    /// </summary>
    /// <returns></returns>
    public static CorsairSessionDetails GetSessionDetails()
    {
        EnsureConnected();
        var details = default(CorsairSessionDetails);
        Methods.CorsairGetSessionDetails(&details).ThrowIfNecessary();
        
        return details;
    }
    
    private static volatile bool _isConnected;
    private static volatile bool _isConnecting;
    private static readonly SemaphoreSlim _connectionChangeAccess = new(1, 1);
    private static CancellationTokenSource? _cts;
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    [SuppressMessage("ReSharper", "UseCollectionExpression")]
    private static void Callback(void* context, CorsairSessionStateChanged* data)
    {
        if (data->state == CorsairSessionState.CSS_Connected)
        {
            _isConnected = true;
            _cts?.Cancel();
        }
            
        Debug.WriteLine($"State Change: [{data->state}]");
        _CorsairActions.ForEach(x => x(*data));

        if (data->state is CorsairSessionState.CSS_Connected or CorsairSessionState.CSS_Connecting)
            return;
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        OnDisconnect?.Invoke();
    }

    public static event Action OnDisconnect;


    internal static bool EnsureConnected()
    {
        if (_isConnected)
            return true;

        if (_isConnecting)
        {
            _connectionChangeAccess.Wait();
            _connectionChangeAccess.Release();
            return _isConnected;
        }
        try
        {
            _connectionChangeAccess.Wait();
            _isConnecting = true;
            StartICUEIfNecessary();

            _cts = new CancellationTokenSource();
            _cts.CancelAfter(TimeSpan.FromSeconds(3));

            Methods.CorsairConnect(&Callback, null);

            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                if (_isConnected)
                    Methods.CorsairDisconnect();
            };

            try
            {
                Task.Delay(-1, _cts.Token).Wait(_cts.Token);
            }
            catch (OperationCanceledException)
            {
            }


            if (!_cts.IsCancellationRequested || _isConnected)
                return true;
            
            if (Preferences.ErrorOnConnectionFailure)
                throw new TimeoutException("Could not access Corsair information");
            return false;
        }
        finally
        {
            _isConnecting = false;
            _connectionChangeAccess.Release();
        }
    }

    private static void StartICUEIfNecessary()
    {
        if (Process.GetProcessesByName("iCUE").Length > 0)
            return;

        StartICUE();
    }

    // We can very likely use COM interop here to read shortcuts in the Start Menu that direct us to ICUE.
    // We can firstly look in some common places for an exe like %ProgramFiles%/Corsair/Corsair iCUE5 Software/iCUE.exe
    internal static void StartICUE()
    {
        if (File.Exists(CommonPaths.ProgramFiles_Corsair_ICUE))
                Process.Start(CommonPaths.ProgramFiles_Corsair_ICUE);
        else if (File.Exists(CommonPaths.ProgramFiles86_Corsair_CUE))
            Process.Start(CommonPaths.ProgramFiles86_Corsair_CUE);
        else
        {
            if (Preferences.ErrorOnConnectionFailure)
                throw new FileNotFoundException("Could not find Corsair iCUE executable");
        }
    }   

    internal static Task<T> EnsureConnected<T>(Func<Task<T>> asyncOperation) => asyncOperation();

    private static readonly List<Action<CorsairSessionStateChanged>> _CorsairActions = [];
    public static event Action<CorsairSessionStateChanged> OnStateChange
    {
        add => _CorsairActions.Add(value);
        remove => _CorsairActions.Remove(value);
    }

    public static IEnumerable<CorsairDeviceInfo> GetDevices(CorsairDeviceType deviceFilter = CorsairDeviceType.CDT_All)
    {
        EnsureConnected();
        var filter = new CorsairDeviceFilter { deviceTypeMask = (int)deviceFilter };


        var devices = new CorsairDeviceInfo[Methods.CORSAIR_DEVICE_COUNT_MAX];
        var size = default(int);
        fixed (CorsairDeviceInfo* device = &devices[0])
            Methods.CorsairGetDevices(&filter, (int)Methods.CORSAIR_DEVICE_COUNT_MAX, device, &size).ThrowIfNecessary();

        Array.Resize(ref devices, size);

        return devices;
    }

}