using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Dawn.CorsairSDK.Bindings;

public unsafe partial struct CorsairEvent
{
    public CorsairEventId id;

    [NativeTypeName("__AnonymousRecord_iCUESDK_L250_C2")]
    public _Anonymous_e__Union Anonymous;

    public ref CorsairDeviceConnectionStatusChangedEvent* deviceConnectionStatusChangedEvent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref this, 1)).Anonymous.deviceConnectionStatusChangedEvent;
    }

    public ref CorsairKeyEvent* keyEvent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref this, 1)).Anonymous.keyEvent;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct _Anonymous_e__Union
    {
        [FieldOffset(0)]
        [NativeTypeName("const CorsairDeviceConnectionStatusChangedEvent *")]
        public CorsairDeviceConnectionStatusChangedEvent* deviceConnectionStatusChangedEvent;

        [FieldOffset(0)]
        [NativeTypeName("const CorsairKeyEvent *")]
        public CorsairKeyEvent* keyEvent;
    }
}
