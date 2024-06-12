using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Corsair.Device.Internal;

internal sealed unsafe class UTF8StringHandle : IDisposable
{
    private MemoryHandle _underlyingStringMemoryHandle;
    private bool _disposed;

    internal UTF8StringHandle(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        _underlyingStringMemoryHandle = new ReadOnlyMemory<byte>(bytes).Pin();
    }

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
        if (_disposed)
            return;
        _disposed = true;

        _underlyingStringMemoryHandle.Dispose();
        GC.SuppressFinalize(this);
    }

    ~UTF8StringHandle() => Dispose();

    public override string ToString() => new(Value);
}
