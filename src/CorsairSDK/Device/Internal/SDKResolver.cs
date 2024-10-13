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
    private static nint _libaryPointer;

    // If the binary exists
    internal static nint CorsairSDKResolver(string libraryname, Assembly assembly, DllImportSearchPath? searchpath)
    {
        if (libraryname is not LIBRARY_BINDING_NAME)
            return 0;

        if (CorsairSDK.AdvancedOptions.CacheNativeSDK && _libaryPointer != 0)
            return _libaryPointer;

        var architecture = RuntimeInformation.OSArchitecture;

        if (architecture is not (Architecture.X64 or Architecture.X86))
            return 0;

        var lib = architecture == Architecture.X64
            ? X64_FILENAME
            : X86_FILENAME;

        if (string.IsNullOrWhiteSpace(lib))
            return 0;

        if (TrySearchForLibrary(lib, out var ptr))
            _libaryPointer = ptr;
        else
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Binaries", lib);
            if (!ExtractBinary(lib, path))
                return 0;

            _libaryPointer = LoadLibrary(path);
        }

        return _libaryPointer;
    }

    private static bool ExtractBinary(string resourceKey, string outputFilePath)
    {
        var asm = Assembly.GetExecutingAssembly();
        var resourcePath = asm.GetManifestResourceNames().FirstOrDefault(x => x.Contains(resourceKey));

        if (string.IsNullOrWhiteSpace(resourcePath))
        {
            Debug.WriteLine($"Could not find resource {resourceKey}", "SDK Resolver");
            return false;
        }

        try
        {
            using var stream = asm.GetManifestResourceStream(resourcePath);

            if (stream is null)
            {
                // If the project is built correctly, this should never happen
                Debug.WriteLine($"Could not find resource {resourcePath}", "SDK Resolver");
                return false;
            }

            using var fs = File.Create(outputFilePath);
            stream.CopyTo(fs);

            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Exception when extracting binary for resource '{resourceKey}': {e}", "SDK Resolver");
            return false;
        }
    }


    private static bool TrySearchForLibrary(string lib, out nint ptr)
    {
        ptr = 0;

        var libPath = Path.Combine("Binaries", lib);

        var appDirLibPath = Path.Combine(AppContext.BaseDirectory, libPath);


        if (File.Exists(appDirLibPath))
        {
            if (!IsSignedAndVerified(appDirLibPath))
                return false;

            ptr = LoadLibrary(appDirLibPath);
            Debug.WriteLineIf(ptr != 0, $"Loaded SDK from [ {appDirLibPath} ]", "SDK Resolver");
            return true;
        }

        if (!File.Exists(libPath) || !IsSignedAndVerified(libPath))
            return false;

        ptr = LoadLibrary(libPath);
        return true;

    }

    private const string CORSAIR_CERTIFICATE_SUBJECT = """
                                                       CN="Corsair Memory, Inc.", O="Corsair Memory, Inc.", L=Fremont, S=California, C=US
                                                       """;
    private static bool IsSignedAndVerified(string path)
    {
        try
        {
            // Signed
            using var signedCertificate = new X509Certificate2(path);

            // Verified
            var isVerified = signedCertificate.Subject == CORSAIR_CERTIFICATE_SUBJECT && signedCertificate.Verify();

            Debug.WriteLine($"SDK Verified [ {isVerified} ]", "SDK Resolver");

            return isVerified;
        }
        catch (CryptographicException e)
        {
            Debug.WriteLine($"SDK Verified [ False ] : {e.Message}", "SDK Resolver");
            return false;
        }
    }

    private static nint LoadLibrary(string path)
    {
        var loaded = NativeLibrary.TryLoad(path, out var handle);

        Debug.WriteLineIf(!loaded, $"Failed to load library [ {path} ]", "SDK Resolver");

        return handle;
    }
}
