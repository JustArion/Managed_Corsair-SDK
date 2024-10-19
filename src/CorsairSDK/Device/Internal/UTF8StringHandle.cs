using System.Buffers;
using System.Diagnostics;
using System.Text;
using Corsair.Threading;

namespace Corsair.Device.Internal;

internal sealed unsafe class UTF8StringHandle(string str) : IDisposable
{
    private MemoryHandle _underlyingStringMemoryHandle = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(str)).Pin();
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
        GC.SuppressFinalize(this);

        Debug.WriteLine($"Cleaned up a string used in Managed -> Native interop | {str}");
    }

    ~UTF8StringHandle() => Dispose();

    public override string ToString() => new(Value);
}
