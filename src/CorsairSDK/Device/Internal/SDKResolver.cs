using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography;

namespace Corsair.Device.Internal;

internal static class SDKResolver
{
    /// This must be consistent with /ClangSharp/ClangSharpArgs.rsp
    private const string LIBRARY_BINDING_NAME = "iCUESDK";

    private const string X86_FILENAME = "iCUESDK_2019.dll";
    private const string X64_FILENAME = "iCUESDK.x64_2019.dll";

    [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
    // If the binary exists
    internal static nint CorsairSDKResolver(string libraryname, Assembly assembly, DllImportSearchPath? searchpath)
    {
        if (libraryname is not LIBRARY_BINDING_NAME)
        #if !NET7_0_OR_GREATER
            return IntPtr.Zero;
        #else
            return nint.Zero;
        #endif

        var architecture = RuntimeInformation.OSArchitecture;

        var lib = architecture switch {
            Architecture.X86 => X86_FILENAME,
            Architecture.X64 => X64_FILENAME,
            _ => throw new PlatformNotSupportedException()
        };


        if (TrySearchForLibrary(lib, architecture, out var ptr))
            return ptr;


        var path = ExtractBinary(lib, Path.Combine(AppContext.BaseDirectory, "Binaries", lib));

        return LoadLibrary(path);
    }

    private static string ExtractBinary(string resourceKey, string outputFilePath)
    {
        var asm = Assembly.GetExecutingAssembly();
        var resourcePath = asm.GetManifestResourceNames().First(x => x.Contains(resourceKey));

        using var stream = asm.GetManifestResourceStream(resourcePath);

        if (stream is null)
            throw new FileNotFoundException($"Could not find resource {resourcePath}");

        using var fs = File.Create(outputFilePath);
        stream.CopyTo(fs);

        return outputFilePath;
    }


    private static bool TrySearchForLibrary(string lib, Architecture architecture, out nint ptr)
    {
        #if !NET7_0_OR_GREATER
        ptr = IntPtr.Zero;
        #else
        ptr = nint.Zero;
        #endif

        var libPath = Path.Combine("Binaries", lib);

        var appDirLibPath = Path.Combine(AppContext.BaseDirectory, libPath);


        // Corsair's DLLs are signed, but a SHA256 is easier for now.

        if (File.Exists(appDirLibPath))
        {
            if (!VerifySHA256(appDirLibPath, architecture))
                return false;

            ptr = NativeLibrary.Load(appDirLibPath);
            Debug.WriteLine($"Loaded SDK from [ {appDirLibPath} ]", "SDK Resolver");
            return true;
        }

        if (!File.Exists(libPath) || !VerifySHA256(libPath, architecture))
            return false;

        ptr = NativeLibrary.Load(libPath);
        Debug.WriteLine($"Loaded SDK from [ {libPath} ]", "SDK Resolver");
        return true;

    }

    [SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
    private static bool VerifySHA256(string path, Architecture architecture) =>
        architecture switch {
            Architecture.X86 => IsValidHash(path, Hashes.X86_SHA256),
            Architecture.X64 => IsValidHash(path, Hashes.X64_SHA256),
            _ => false
        };

    private static bool IsValidHash(string path, string expectedHash)
    {
        byte[] sha256;
        using (var hFile = File.Open(path, FileMode.Open, FileAccess.Read))
            sha256 = SHA256.Create().ComputeHash(hFile);

        var actualHash = BitConverter.ToString(sha256).Replace("-", "").ToLower();

        return expectedHash == actualHash;
    }

    private static nint LoadLibrary(string path)
    {
        return NativeLibrary.Load(path);
    }
}
