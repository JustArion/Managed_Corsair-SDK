using System.Diagnostics;

namespace Corsair.Device.Internal;

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

    // This acts like a sbyte* but with the benefit of the conversion only happening when needed. Minimizing the time the handle is exposed as a sbyte* when held as a stack variable.
    // This would normally happen if you have a sbyte* to the string handle, some gc pressure causes a collection and some time passes after you have your ToPointer as a astack variable.
    // This limits the exposure of sbyte* until necessary.
    internal static UTF8StringHandle ToPointer(string str) => new(str);

    /// <summary>
    /// Copies the array contents and returns a managed copy
    /// </summary>
    internal static T[] ToArray<T>(T* arrayPtr, uint size) where T : unmanaged
    {
        var result = new T[size];

        var array = new ReadOnlySpan<T>(arrayPtr, (int)size);

        if (array.TryCopyTo(result))
            return result;

        Debug.Fail("Failed to copy array");
        return [];
    }

    // We don't need a memcpy here since the strings are being created in the ToString method
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/nativeaot/System.Private.CoreLib/src/Internal/Runtime/CompilerHelpers/StartupCode/StartupCodeHelpers.Extensions.cs#L32
    internal static string[] ToArray(sbyte** stringArray, uint size)
    {
        var result = new string[size];

        for (var i = 0; i < size; i++)
            result[i] = ToString(stringArray[i]);

        return result;
    }

    internal static string ToString(sbyte* ptr) => new(ptr);
}
