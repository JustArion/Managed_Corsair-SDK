namespace Dawn.Libs.Corsair.SDK.LowLevel;

public static class ArchitectureConstants
{
    #if X64
    public const string iCUESDK = @"Native\Libs\iCUESDK.x64_2019.dll";
    #else
    public const string iCUESDK = @"Native\Libs\iCUESDK_2019.dll";
    #endif
}