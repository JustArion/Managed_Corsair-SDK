// Thanks to https://github.com/ophura for the String Marshaller!
// https://gist.github.com/ophura/12dcb3278d78260601d83bc53c7b228d
using System.Buffers;
using System.Reflection;

internal static unsafe class MbcsStringMarshaller
{
    private delegate int GetAnsiStringByteCount(ReadOnlySpan<char> src);
    private delegate void GetAnsiStringBytes(ReadOnlySpan<char> src, Span<byte> dst);

    private static readonly GetAnsiStringByteCount s_getAnsiStringByteCount;
    private static readonly GetAnsiStringBytes s_getAnsiStringBytes;
    private static IMemoryOwner<byte>? s_lastManager;

    static MbcsStringMarshaller()
    {
        const BindingFlags internalStatic = BindingFlags.NonPublic | BindingFlags.Static;

        var size = typeof(Marshal).GetMethod(nameof(GetAnsiStringByteCount), internalStatic);
        var atob = typeof(Marshal).GetMethod(nameof(GetAnsiStringBytes), internalStatic);

        s_getAnsiStringByteCount = size!.CreateDelegate<GetAnsiStringByteCount>();
        s_getAnsiStringBytes = atob!.CreateDelegate<GetAnsiStringBytes>();
    }

    public static MbcsStringMemory ConvertToUnmanaged(ReadOnlySpan<char> managed)
    {
        var exactByteCount = GetAnsiStringByteCount(managed);

        s_lastManager = MemoryPool<byte>.Shared.Rent(exactByteCount);

        GetAnsiStringBytes(managed, s_lastManager.Memory.Span);

        return new(in s_lastManager);

        static int GetAnsiStringByteCount(ReadOnlySpan<char> src) =>
            s_getAnsiStringByteCount(src);

        static void GetAnsiStringBytes(ReadOnlySpan<char> src, Span<byte> dst) =>
            s_getAnsiStringBytes(src, dst);
    }

    public static string ConvertToManaged(sbyte* unmanaged) => new(unmanaged);

#if NET7_0_OR_GREATER
    public readonly ref struct MbcsStringMemory
    {
        private readonly ref readonly IMemoryOwner<byte> _manager;
        private readonly sbyte* _pointer;

        internal MbcsStringMemory(ref readonly IMemoryOwner<byte> manager)
        {
            _manager = ref manager;

            fixed (byte* pointer = _manager.Memory.Span)
            {
                _pointer = (sbyte*)pointer;
            }
        }

        public readonly sbyte* Pointer => _pointer;

        public readonly void Dispose() => _manager.Dispose();
    }
#else // !NET7_0_OR_GREATER
    public sealed class MbcsStringMemory : IDisposable
    {
        private readonly IMemoryOwner<byte> _manager;
        private readonly sbyte* _pointer;

        internal MbcsStringMemory(ref readonly IMemoryOwner<byte> manager)
        {
            _manager = manager;

            fixed (byte* pointer = _manager.Memory.Span)
            {
                _pointer = (sbyte*)pointer;
            }
        }

        public sbyte* Pointer => _pointer;

        public void Dispose() => _manager.Dispose();
    }
#endif // NET7_0_OR_GREATER
}
