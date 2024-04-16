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

        var architecture = RuntimeInformation.OSArchitecture;

        var lib = architecture switch {
            Architecture.X86 => "iCUESDK_2019.dll",
            Architecture.X64 => "iCUESDK.x64_2019.dll",
            _ => string.Empty
        };


        var libPath = Path.Combine("Binaries", lib);

        var appDirLibPath = Path.Combine(AppContext.BaseDirectory, libPath);

        if (File.Exists(appDirLibPath))
            return NativeLibrary.Load(appDirLibPath);

        return File.Exists(libPath)
            ? NativeLibrary.Load(libPath)
            : IntPtr.Zero;
    }
}
