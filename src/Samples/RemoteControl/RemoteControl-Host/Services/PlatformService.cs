namespace Dawn.Apps.RemoteControlHost.Services;

using CorsairSDK.Rewrite.Lighting;
using CorsairSDK.Rewrite.Lighting.Contracts;
using Rewrite;

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