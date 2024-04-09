using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Dawn.CorsairSDK.Rewrite.Device.Internal;

internal static class SDKResolver
{
    /// This must be consistent with /ClangSharp/ClangSharpArgs.rsp
    private const string LIBRARY_BINDING_NAME = "iCUESDK";

    [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
    internal static nint CorsairSDKResolver(string libraryname, Assembly assembly, DllImportSearchPath? searchpath)
    {
        if (libraryname is not LIBRARY_BINDING_NAME)
            return IntPtr.Zero;

        return RuntimeInformation.OSArchitecture switch
        {
            Architecture.X86 => NativeLibrary.Load(Path.Combine("Binaries", "iCUESDK_2019.dll")),
            Architecture.X64 => NativeLibrary.Load(Path.Combine("Binaries", "iCUESDK.x64_2019.dll")),
            _ => throw new PlatformNotSupportedException()
        };
    }
}
