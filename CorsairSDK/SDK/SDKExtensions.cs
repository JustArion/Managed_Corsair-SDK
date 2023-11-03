namespace Dawn.CorsairSDK;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LowLevel;

internal static class SDKExtensions
{
    [StackTraceHidden]
    internal static void ThrowIfNecessary(this CorsairError error, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (error == CorsairError.CE_Success)
            return;

        throw new Exception($"{error.ToString()} in {memberName}:{lineNumber} at {filePath}");
    }

    internal static string ToAnsiString(this nint ptr) => Marshal.PtrToStringAnsi(ptr)!;

}