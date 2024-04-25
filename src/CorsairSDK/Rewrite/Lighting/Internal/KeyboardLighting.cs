using Dawn.CorsairSDK.Bindings;

namespace Dawn.CorsairSDK.Rewrite.Lighting.Internal;

using System.Diagnostics;
using Connection.Internal.Contracts;
using Device;
using Lighting.Contracts;

internal class KeyboardLighting : IKeyboardLighting, IDisposable
{
    private readonly IDeviceConnectionHandler _connectionHandler;
    private readonly CorsairDevice? _underlyingDevice;

    internal KeyboardLighting(IDeviceConnectionHandler connectionHandler)
    {
        _connectionHandler = connectionHandler;

        _colorController = new KeyboardColorController(_connectionHandler);

        Effects = new EffectController(_colorController);
    }

    internal KeyboardLighting(IDeviceConnectionHandler connectionHandler, CorsairDevice device)
    {
        _connectionHandler = connectionHandler;

        _colorController = new KeyboardColorController(_connectionHandler);

        Effects = new EffectController(_colorController);
    }


    public bool TryInitialize(AccessLevel accessLevel = AccessLevel.Exclusive)
    {
        var connected =  _connectionHandler.Connect(DeviceReconnectPolicy.Default);

        var initialized =  connected && OnConnectionEstablished(accessLevel, _underlyingDevice);

        var grammar = initialized ? "is" : "is not";
        // Device is initialized
        // Device is not initialized
        Debug.WriteLine($"Device {grammar} initialized", "Keyboard Lighting");


        LogSessionInfo();
        return initialized;
    }

    [Conditional("DEBUG")]
    private void LogSessionInfo()
    {
        var sessionDetails = Dawn.Rewrite.CorsairSDK._connectionHandler._deviceConnection.GetConnectionDetails();

        Debug.WriteLine($"Client Version: {VersionToString(sessionDetails.clientVersion)}", "Session Analytics");
        Debug.WriteLine($"Server Version: {VersionToString(sessionDetails.serverVersion)}", "Session Analytics");
        Debug.WriteLine($"Sever-Host Version: {VersionToString(sessionDetails.serverHostVersion)}", "Session Analytics");

        return;
        string VersionToString(CorsairVersion version) => $"{version.major}.{version.minor}.{version.patch}";
    }

    private bool OnConnectionEstablished(AccessLevel accessLevel, CorsairDevice? keyboard = null)
    {
        keyboard ??= Dawn.Rewrite.CorsairSDK.GetDevices(DeviceType.Keyboard).FirstOrDefault();

        if (keyboard == null)
        {
            Debug.WriteLine("iCUE could connect, but did not detect a Corsair Keyboard connected to this system");
            return false;
        }

        _colorController.SetContext(keyboard, accessLevel);

        Debug.WriteLine($"Lighting established for Device 'Corsair {keyboard.Model}'", "Keyboard Lighting");
        return true;
    }


    private readonly KeyboardColorController _colorController;
    public IColorController Colors => _colorController;
    public IEffectController Effects { get; }

    public void Shutdown()
    {
        Colors.Dispose();
        // TODO: Uncomment this when Effects is implemented
        Effects?.Dispose();
        _connectionHandler.Disconnect();
    }

    public void Dispose() => Shutdown();
}
