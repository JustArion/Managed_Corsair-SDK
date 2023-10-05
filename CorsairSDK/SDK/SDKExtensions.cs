namespace Dawn.CorsairSDK;

using System.Runtime.InteropServices;
using LowLevel;

internal static class SDKExtensions
{
    internal static void ThrowIfNecessary(this CorsairError error)
    {
        if (error == CorsairError.CE_Success)
            return;

        throw new Exception(error.ToString());
    }

    internal static string ToAnsiString(this nint ptr) => Marshal.PtrToStringAnsi(ptr)!;

}