using Dawn.CorsairSDK.Rewrite.Device.Internal;
using Dawn.CorsairSDK.Rewrite.Threading;

namespace Dawn.CorsairSDK.Rewrite.Connection.Internal;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Contracts;
using Device.Internal.Contracts;
using Bindings;

internal class DeviceConnectionHandler : IDeviceConnectionHandler
{
    internal readonly DeviceConnection _deviceConnection;
    // internal readonly DeviceReconnectHandler _reconnectHandler = new();
    private TaskCompletionSource _connectionWaitHandler;

    internal DeviceConnectionHandler(DeviceConnection? underlyingConnection = null)
    {
        _connectionWaitHandler = new();

        _deviceConnection = underlyingConnection ?? new DeviceConnection();
        _deviceConnection.SessionStateChanged += OnSessionStateChangedAsync;
    }

    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    private void OnSessionStateChangedAsync(object? sender, CorsairSessionState e)
    {
        LogStateChange(e);
        switch (e)
        {
            case CorsairSessionState.CSS_Timeout:
            case CorsairSessionState.CSS_ConnectionRefused:
            case CorsairSessionState.CSS_ConnectionLost:
                ChangeConnectionState(ConnectionState.Disconnected);
                break;
            case CorsairSessionState.CSS_Invalid:
                break;
            case CorsairSessionState.CSS_Closed:
                ChangeConnectionState(ConnectionState.Disconnected);
                break;
            case CorsairSessionState.CSS_Connecting:
                ChangeConnectionState(ConnectionState.Connecting);
                break;
            case CorsairSessionState.CSS_Connected:
                ChangeConnectionState(ConnectionState.Connected);
                break;
        }
    }

    private void ChangeConnectionState(ConnectionState state)
    {
        if (ConnectionState == state)
            return;
        _connectionState.GetAndSet((int)state);

        switch (state)
        {
            case ConnectionState.Connected:
                SetAndRefreshWaitHandler();
                break;
            case ConnectionState.Disconnected:
                SetAndRefreshWaitHandler();
                Debug.WriteLine("Lost connection, reconnecting...", "Device Connection Handler");
                // Task.Run(()=> _reconnectHandler.RequestReconnection(TryReconnect));
                break;
            case ConnectionState.Connecting:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

    }

    private void SetAndRefreshWaitHandler()
    {
        _connectionWaitHandler?.SetResult();
        _connectionWaitHandler = new();
    }


    [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
    public bool Connect(DeviceReconnectPolicy? reconnectPolicy = null)
    {
        if (ConnectionState == ConnectionState.Connecting)
            _connectionWaitHandler.Task.Wait();
        if (ConnectionState == ConnectionState.Connected)
            return true;

        // if (reconnectPolicy != null)
            // _reconnectHandler.ReconnectPolicy = reconnectPolicy;
            if (reconnectPolicy != null)
                ReconnectPolicy = reconnectPolicy;


        if (!_deviceConnection.Connect())
            return false;

        Wait(ReconnectPolicy.GetMaxWaitTime());
        return ConnectionState == ConnectionState.Connected;
    }

    // public DeviceReconnectPolicy ReconnectPolicy
    // {
    //     get => _reconnectHandler.ReconnectPolicy;
    //     set => _reconnectHandler.ReconnectPolicy = value;
    // }

    public DeviceReconnectPolicy ReconnectPolicy { get; set; } = DeviceReconnectPolicy.Default;


    [StackTraceHidden]
    public void Wait(CancellationToken token = default)
    {
        if (ConnectionState == ConnectionState.Connected)
            return;

        try
        {
            _connectionWaitHandler!.Task.Wait(token);

        }
        catch (OperationCanceledException) { }

    }

    private AtomicInteger _connectionState;
    public ConnectionState ConnectionState => (ConnectionState)_connectionState.Value;

    public void Disconnect() =>
        _deviceConnection.Disconnect();

    [Conditional("DEBUG")]
    private static void LogStateChange(CorsairSessionState state) => Debug.WriteLine(state, "Device State Change");
}
