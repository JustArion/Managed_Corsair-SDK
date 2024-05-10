using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Corsair.Device.Internal;

internal unsafe class UTF8StringHandle : IDisposable
{
    // We need this as a reference to the buffer variable since a pointer doesn't serve as a reference and may be GCed
    private byte[] _buffer;
    private sbyte* _handle;

    internal UTF8StringHandle(string str)
    {
        var buffer = Encoding.UTF8.GetBytes(str);
        _buffer = buffer;

        fixed (byte* ptr = _buffer)
            _handle = (sbyte*)ptr;
    }

    public ref readonly sbyte* Handle => ref _handle;

    public void Dispose()
    {
        _buffer = null!;
        _handle = null;
        GC.SuppressFinalize(this);
    }

    ~UTF8StringHandle() => Dispose();
}
