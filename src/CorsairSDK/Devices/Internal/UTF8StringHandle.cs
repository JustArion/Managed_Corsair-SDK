using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Corsair.Device.Internal;

internal unsafe class UTF8StringHandle : IDisposable
{
    private GCHandle _handle;

    internal UTF8StringHandle(string str)
    {
        var buffer = Encoding.UTF8.GetBytes(str);
        _handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    }

    public sbyte* Handle => (sbyte*)_handle.AddrOfPinnedObject();

    public static implicit operator sbyte*(UTF8StringHandle handle) => handle.Handle;

    public void Dispose()
    {
        _handle.Free();
        GC.SuppressFinalize(this);
    }

    ~UTF8StringHandle() => Dispose();
}
