using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Corsair.Device.Internal;

internal sealed unsafe class UTF8StringHandle : IDisposable
{
    private readonly sbyte* _value;
    private bool _disposed;

    internal UTF8StringHandle(ReadOnlySpan<char> str)
    {
        var byteLength = Encoding.UTF8.GetByteCount(str);

        _value = (sbyte*)NativeMemory.Alloc((nuint)byteLength);
        _value[byteLength] = 0; // Ensure Null-Terminated

        Encoding.UTF8.GetBytes(str, new Span<byte>(_value, byteLength));
    }

    public ref readonly sbyte* Value
    {
        get
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UTF8StringHandle));

            return ref _value;
        }
    }

    public static implicit operator sbyte*(UTF8StringHandle handle) => handle.Value;

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        NativeMemory.Free(_value);
        GC.SuppressFinalize(this);
    }

    ~UTF8StringHandle() => Dispose();
}
