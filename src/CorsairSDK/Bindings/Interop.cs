using System.Runtime.InteropServices;

namespace Corsair.Bindings;

public static unsafe partial class Interop
{
    [NativeTypeName("const unsigned int")]
    public const uint CORSAIR_STRING_SIZE_S = 64;

    [NativeTypeName("const unsigned int")]
    public const uint CORSAIR_STRING_SIZE_M = 128;

    [NativeTypeName("const unsigned int")]
    public const uint CORSAIR_LAYER_PRIORITY_MAX = 255;

    [NativeTypeName("const unsigned int")]
    public const uint CORSAIR_DEVICE_COUNT_MAX = 64;

    [NativeTypeName("const unsigned int")]
    public const uint CORSAIR_DEVICE_LEDCOUNT_MAX = 512;

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairConnect", ExactSpelling = true)]
    public static extern CorsairError Connect([NativeTypeName("CorsairSessionStateChangedHandler")] delegate* unmanaged[Cdecl]<void*, CorsairSessionStateChanged*, void> onStateChanged, void* context);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairGetSessionDetails", ExactSpelling = true)]
    public static extern CorsairError GetSessionDetails(CorsairSessionDetails* details);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairDisconnect", ExactSpelling = true)]
    public static extern CorsairError Disconnect();

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairGetDevices", ExactSpelling = true)]
    public static extern CorsairError GetDevices([NativeTypeName("const CorsairDeviceFilter *")] CorsairDeviceFilter* filter, int sizeMax, CorsairDeviceInfo* devices, int* size);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairGetDeviceInfo", ExactSpelling = true)]
    public static extern CorsairError GetDeviceInfo([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairDeviceInfo* deviceInfo);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairGetLedPositions", ExactSpelling = true)]
    public static extern CorsairError GetLedPositions([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, int sizeMax, CorsairLedPosition* ledPositions, int* size);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairSubscribeForEvents", ExactSpelling = true)]
    public static extern CorsairError SubscribeForEvents([NativeTypeName("CorsairEventHandler")] delegate* unmanaged[Cdecl]<void*, CorsairEvent*, void> onEvent, void* context);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairUnsubscribeFromEvents", ExactSpelling = true)]
    public static extern CorsairError UnsubscribeFromEvents();

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairConfigureKeyEvent", ExactSpelling = true)]
    public static extern CorsairError ConfigureKeyEvent([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, [NativeTypeName("const CorsairKeyEventConfiguration *")] CorsairKeyEventConfiguration* config);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairGetDevicePropertyInfo", ExactSpelling = true)]
    public static extern CorsairError GetDevicePropertyInfo([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairDevicePropertyId propertyId, [NativeTypeName("unsigned int")] uint index, CorsairDataType* dataType, [NativeTypeName("unsigned int *")] uint* flags);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairReadDeviceProperty", ExactSpelling = true)]
    public static extern CorsairError ReadDeviceProperty([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairDevicePropertyId propertyId, [NativeTypeName("unsigned int")] uint index, CorsairProperty* property);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairWriteDeviceProperty", ExactSpelling = true)]
    public static extern CorsairError WriteDeviceProperty([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairDevicePropertyId propertyId, [NativeTypeName("unsigned int")] uint index, [NativeTypeName("const CorsairProperty *")] CorsairProperty* property);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairFreeProperty", ExactSpelling = true)]
    public static extern CorsairError FreeProperty(CorsairProperty* property);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairSetLedColors", ExactSpelling = true)]
    public static extern CorsairError SetLedColors([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, int size, [NativeTypeName("const CorsairLedColor *")] CorsairLedColor* ledColors);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairSetLedColorsBuffer", ExactSpelling = true)]
    public static extern CorsairError SetLedColorsBuffer([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, int size, [NativeTypeName("const CorsairLedColor *")] CorsairLedColor* ledColors);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairSetLedColorsFlushBufferAsync", ExactSpelling = true)]
    public static extern CorsairError SetLedColorsFlushBufferAsync([NativeTypeName("CorsairAsyncCallback")] delegate* unmanaged[Cdecl]<void*, CorsairError, void> callback, void* context);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairGetLedColors", ExactSpelling = true)]
    public static extern CorsairError GetLedColors([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, int size, CorsairLedColor* ledColors);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairSetLayerPriority", ExactSpelling = true)]
    public static extern CorsairError SetLayerPriority([NativeTypeName("unsigned int")] uint priority);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairGetLedLuidForKeyName", ExactSpelling = true)]
    public static extern CorsairError GetLedLuidForKeyName([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, [NativeTypeName("char")] sbyte keyName, [NativeTypeName("CorsairLedLuid *")] uint* ledId);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairRequestControl", ExactSpelling = true)]
    public static extern CorsairError RequestControl([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairAccessLevel accessLevel);

    [DllImport("iCUESDK", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CorsairReleaseControl", ExactSpelling = true)]
    public static extern CorsairError ReleaseControl([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId);
}
