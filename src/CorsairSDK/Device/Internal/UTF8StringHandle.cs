using System.Buffers;
using System.Diagnostics;
using System.Text;
using Corsair.Threading;

namespace Corsair.Device.Internal;

internal sealed unsafe class UTF8StringHandle(string str) : IDisposable
{
    private static MemoryHandle ToMemoryHandle(string s)
    {
        var buffer = Encoding.UTF8.GetBytes(s);

        var arrayPtr = Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(buffer));
        var gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        return new MemoryHandle(arrayPtr, gcHandle);
    }

    private MemoryHandle _underlyingStringMemoryHandle = ToMemoryHandle(str);

    private AtomicBoolean _disposed;

    public sbyte* Value
    {
        get
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UTF8StringHandle));

            return (sbyte*)_underlyingStringMemoryHandle.Pointer;
        }
    }

    public static implicit operator sbyte*(UTF8StringHandle handle) => handle.Value;

    public void Dispose()
    {
        if (!_disposed.CompareAndSet(false, true))
            return;

        _underlyingStringMemoryHandle.Dispose();
        _underlyingStringMemoryHandle = default;
        GC.SuppressFinalize(this);

        Debug.WriteLine($"Cleaned up a string used in Managed -> Native interop | {str}");
    }

    ~UTF8StringHandle() => Dispose();

    public override string ToString() => str;
}
