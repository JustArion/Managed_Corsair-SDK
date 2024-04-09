using Dawn.CorsairSDK.Rewrite.Exceptions;

namespace Dawn.CorsairSDK.Rewrite.Device.Internal;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Bindings;

internal static unsafe class SDKExtensions
{
    [StackTraceHidden]
    internal static void ThrowIfNecessary(this CorsairError error, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (error == CorsairError.CE_Success)
            return;

        throw new CorsairException($"{error.ToString()} in {memberName} at {filePath}:{lineNumber}");
    }

    internal static string ToAnsiString(this nint ptr) => Marshal.PtrToStringAnsi(ptr)!;

    internal static string ToAnsiString(sbyte* ptr) => Marshal.PtrToStringAnsi(new IntPtr(ptr))!;
}
