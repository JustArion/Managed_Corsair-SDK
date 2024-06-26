using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

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


        if (TrySearchForLibrary(lib, out var ptr))
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


    private static bool TrySearchForLibrary(string lib, out nint ptr)
    {
        #if !NET7_0_OR_GREATER
        ptr = IntPtr.Zero;
        #else
        ptr = nint.Zero;
        #endif

        var libPath = Path.Combine("Binaries", lib);

        var appDirLibPath = Path.Combine(AppContext.BaseDirectory, libPath);


        if (File.Exists(appDirLibPath))
        {
            if (!IsSignedAndVerified(appDirLibPath))
                return false;

            ptr = LoadLibrary(appDirLibPath);
            Debug.WriteLine($"Loaded SDK from [ {appDirLibPath} ]", "SDK Resolver");
            return true;
        }

        if (!File.Exists(libPath) || !IsSignedAndVerified(libPath))
            return false;

        ptr = LoadLibrary(libPath);
        return true;

    }

    private static bool IsSignedAndVerified(string path)
    {
        try
        {
            // Signed
            using var cert = X509Certificate.CreateFromSignedFile(path);

            // Verified
            return cert.Subject == """CN="Corsair Memory, Inc.", O="Corsair Memory, Inc.", L=Fremont, S=California, C=US""";
        }
        catch (CryptographicException)
        {
            return false;
        }
    }

    private static nint LoadLibrary(string path)
    {
        var loaded = NativeLibrary.TryLoad(path, out var handle);

        if (loaded)
            Debug.WriteLine($"Loaded SDK from [ {path} ]", "SDK Resolver");

        return handle;
    }
}
