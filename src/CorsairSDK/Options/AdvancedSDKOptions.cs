namespace Corsair.Options;

/// <summary>
/// Options that should be set before interacting with the SDK, 99% of the time you won't need to touch this.
/// </summary>
public sealed class AdvancedSDKOptions
{
    /// <summary>
    /// If you intend to manually unload the native binary "iCUESDK.x64_2019.dll" or "iCUESDK_2019.dll" and continue using the SDK, set this property to false.
    /// This option is used in <see cref="T:Corsair.Device.Internal.SDKResolver"/> for handling import resolutions.
    /// </summary>
    public bool CacheNativeSDK { get; set; } = true;
}
