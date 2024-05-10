using System.Diagnostics.CodeAnalysis;

namespace Corsair.Device.Internal;

internal unsafe class UTF8StringHandle : IDisposable
{
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    // We need this as a reference to the buffer variable since a pointer doesn't serve as a reference and may be GCed
    private readonly byte[] _buffer;
    private sbyte* _handle;

    internal UTF8StringHandle(byte[] buffer)
    {
        _buffer = buffer;

        fixed (byte* ptr = _buffer)
            _handle = (sbyte*)ptr;
    }

    public ref readonly sbyte* Handle => ref _handle;

    public static implicit operator sbyte*(UTF8StringHandle handle) => handle._handle;

    public void Dispose()
    {
        _handle = null;
        GC.SuppressFinalize(this);
    }

    ~UTF8StringHandle() => Dispose();
}
