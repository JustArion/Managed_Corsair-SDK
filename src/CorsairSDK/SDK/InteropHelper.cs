namespace Dawn.CorsairSDK;

using System.Runtime.InteropServices;

internal static class InteropHelper
{
    // int
    public static unsafe T[] ToManagedArray<T>(T* arrayPtr, int size) where T : unmanaged
    {
        var result = new T[size];
        
        for (var i = 0; i < size; i++)
        {
            // We increment i to the ptr, an operator override by the BCL increments it by sizeof(T), then we dereference the new address.
            result[i] = *(arrayPtr + i);
        }
        
        return result;
    }
    
    // uint
    public static unsafe T[] ToManagedArray<T>(T* arrayPtr, uint size) where T : unmanaged
    {
        var result = new T[size];

        for (var i = 0; i < size; i++)
        {
            result[i] = *(arrayPtr + i);
        }

        return result;
    }

    public static unsafe string[] ToManagedArray(sbyte** arrayPtr, uint size)
    {
        var result = new string[size];

        for (var i = 0; i < size; i++)
        {
            result[i] = Marshal.PtrToStringAnsi((nint)(*(arrayPtr + i)))!;
        }

        return result;
    }
    
    public static unsafe T*[] ToManagedArray<T>(T** arrayPtr, uint size) where T : unmanaged
    {
        var result = new T*[size];

        for (var i = 0; i < size; i++)
        {
            result[i] = *(arrayPtr + i);
        }

        return result;
    }
}