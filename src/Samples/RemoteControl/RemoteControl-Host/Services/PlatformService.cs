namespace Corsair.Apps.RemoteControlHost.Services;

using Lighting;
using Lighting.Contracts;

public class PlatformService
{
    private readonly IKeyboardLighting _keyboard;
    public PlatformService(ILogger<PlatformService> logger)
    {
        _keyboard = CorsairSDK.KeyboardLighting;
        _keyboard.TryInitialize(AccessLevel.Shared);
        
        logger.LogInformation("Platform Service established");
    }

    public IColorController Colors => _keyboard.Colors;
}