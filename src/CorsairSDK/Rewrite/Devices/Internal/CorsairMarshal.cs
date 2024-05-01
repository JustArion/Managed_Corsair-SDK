using System.Diagnostics;
using System.Reactive.Disposables;

namespace Dawn.CorsairSDK.Rewrite.Device.Internal;

using System.Runtime.CompilerServices;

internal static unsafe class CorsairMarshal
{
    internal static T CopyUnmanagedType<T>(scoped in T type) where T : unmanaged
    {
        var newObject = new T();

        fixed (T* obj = &type)
            Unsafe.Copy(ref newObject, obj);

        return newObject;
    }

    internal static IDisposable StringToAnsiPointer(string? st, out sbyte* id)
    {
        var str = Marshal.StringToHGlobalAnsi(st);

        id = (sbyte*)str;

        return Disposable.Create(str, Marshal.FreeHGlobal);
    }

    /// <summary>
    /// Copies the array contents and returns a managed copy
    /// </summary>
    public static T[] ToArray<T>(T* arrayPtr, uint size) where T : unmanaged
    {
        var result = new T[size];

        var array = new ReadOnlySpan<T>(arrayPtr, (int)size);

        if (array.TryCopyTo(result))
            return result.ToArray();

        Debug.WriteLine("Failed to copy array");
        return [];
    }

    // We don't need a memcpy here since the strings are being created in the ToString method
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/nativeaot/System.Private.CoreLib/src/Internal/Runtime/CompilerHelpers/StartupCode/StartupCodeHelpers.Extensions.cs#L32
    internal static string[] ToArray(sbyte** arrayPtr, uint size)
    {
        var result = new string[size];

        for (var i = 0; i < size; i++)
            result[i] = ToString(arrayPtr[i]);

        return result;
    }

    internal static string ToString(sbyte* ptr) => new(ptr);

}
