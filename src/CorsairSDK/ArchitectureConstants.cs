namespace Dawn.CorsairSDK.LowLevel;

public static class ArchitectureConstants
{
    private const string DIRECTORY_PATH = @"Binaries\";
    #if X64
    public const string iCUESDK = DIRECTORY_PATH + "iCUESDK.x64_2019.dll";
    #else
    public const string iCUESDK = DIRECTORY_PATH + "iCUESDK_2019.dll";
    #endif
}