// ReSharper disable All
using System.Runtime.InteropServices;

namespace Dawn.CorsairSDK.LowLevel
{
    using System.Diagnostics.CodeAnalysis;

    public enum CorsairError
    {
        CE_Success = 0,
        CE_NotConnected = 1,
        CE_NoControl = 2,
        CE_IncompatibleProtocol = 3,
        CE_InvalidArguments = 4,
        CE_InvalidOperation = 5,
        CE_DeviceNotFound = 6,
        CE_NotAllowed = 7,
    }

    public enum CorsairSessionState
    {
        CSS_Invalid = 0,
        CSS_Closed = 1,
        CSS_Connecting = 2,
        CSS_Timeout = 3,
        CSS_ConnectionRefused = 4,
        CSS_ConnectionLost = 5,
        CSS_Connected = 6,
    }

    public enum CorsairDeviceType
    {
        CDT_Unknown = 0x0000,
        CDT_Keyboard = 0x0001,
        CDT_Mouse = 0x0002,
        CDT_Mousemat = 0x0004,
        CDT_Headset = 0x0008,
        CDT_HeadsetStand = 0x0010,
        CDT_FanLedController = 0x0020,
        CDT_LedController = 0x0040,
        CDT_MemoryModule = 0x0080,
        CDT_Cooler = 0x0100,
        CDT_Motherboard = 0x0200,
        CDT_GraphicsCard = 0x0400,
        CDT_Touchbar = 0x0800,
        CDT_All = unchecked((int)(0xFFFFFFFF)),
    }

    public enum CorsairEventId
    {
        CEI_Invalid = 0,
        CEI_DeviceConnectionStatusChangedEvent = 1,
        CEI_KeyEvent = 2,
    }

    public enum CorsairMacroKeyId
    {
        CMKI_Invalid = 0,
        CMKI_1 = 1,
        CMKI_2 = 2,
        CMKI_3 = 3,
        CMKI_4 = 4,
        CMKI_5 = 5,
        CMKI_6 = 6,
        CMKI_7 = 7,
        CMKI_8 = 8,
        CMKI_9 = 9,
        CMKI_10 = 10,
        CMKI_11 = 11,
        CMKI_12 = 12,
        CMKI_13 = 13,
        CMKI_14 = 14,
        CMKI_15 = 15,
        CMKI_16 = 16,
        CMKI_17 = 17,
        CMKI_18 = 18,
        CMKI_19 = 19,
        CMKI_20 = 20,
    }

    /// contains list of properties identifiers which can be 
    public enum CorsairDevicePropertyId
    {
        CDPI_Invalid = 0,							/// dummy value
        CDPI_PropertyArray = 1,						/// array of CorsairDevicePropertyId members supported by device
        CDPI_MicEnabled = 2,						/// indicates Mic state (On or Off); used for headset, headset stand
        CDPI_SurroundSoundEnabled = 3,				/// indicates Surround Sound state (On or Off); used for headset, headset stand
        CDPI_SidetoneEnabled = 4,					/// indicates Sidetone state (On or Off); used for headset (where applicable)
        CDPI_EqualizerPreset = 5,					/// the number of active equalizer preset (integer, 1 - 5); used for headset, headset stand
        CDPI_PhysicalLayout = 6,					/// keyboard physical layout (see CorsairPhysicalLayout for valid values); used for keyboard
        CDPI_LogicalLayout = 7,						/// keyboard logical layout (see CorsairLogicalLayout for valid values); used for keyboard
        CDPI_MacroKeyArray = 8,						/// array of programmable G, M or S keys on device
        CDPI_BatteryLevel = 9,						/// battery level (0 - 100); used for wireless devices
        CDPI_ChannelLedCount = 10,					/// total number of LEDs connected to the channel
        CDPI_ChannelDeviceCount = 11,				/// number of LED-devices (fans, strips, etc.) connected to the channel which is controlled by the DIY device
        CDPI_ChannelDeviceLedCountArray = 12,		/// array of integers, each element describes the number of LEDs controlled by the channel device
        CDPI_ChannelDeviceTypeArray = 13,			/// array of CorsairChannelDeviceType members, each element describes the type of the channel device
    }

    public enum CorsairDataType
    {
        CT_Boolean = 0,
        CT_Int32 = 1,
        CT_Float64 = 2,
        CT_String = 3,
        CT_Boolean_Array = 16,
        CT_Int32_Array = 17,
        CT_Float64_Array = 18,
        CT_String_Array = 19,
    }

    public enum CorsairPropertyFlag
    {
        CPF_None = 0x00,
        CPF_CanRead = 0x01,
        CPF_CanWrite = 0x02,
        CPF_Indexed = 0x04,
    }

    public enum CorsairPhysicalLayout
    {
        CPL_Invalid = 0,
        CPL_US = 1,
        CPL_UK = 2,
        CPL_JP = 3,
        CPL_KR = 4,
        CPL_BR = 5,
    }

    public enum CorsairLogicalLayout
    {
        CLL_Invalid = 0,
        CLL_US_Int = 1,
        CLL_NA = 2,
        CLL_EU = 3,
        CLL_UK = 4,
        CLL_BE = 5,
        CLL_BR = 6,
        CLL_CH = 7,
        CLL_CN = 8,
        CLL_DE = 9,
        CLL_ES = 10,
        CLL_FR = 11,
        CLL_IT = 12,
        CLL_ND = 13,
        CLL_RU = 14,
        CLL_JP = 15,
        CLL_KR = 16,
        CLL_TW = 17,
        CLL_MEX = 18,
    }

    public enum CorsairChannelDeviceType
    {
        CCDT_Invalid = 0,
        CCDT_HD_Fan = 1,
        CCDT_SP_Fan = 2,
        CCDT_LL_Fan = 3,
        CCDT_ML_Fan = 4,
        CCDT_QL_Fan = 5,
        CCDT_8LedSeriesFan = 6,
        CCDT_Strip = 7,
        CCDT_DAP = 8,
        CCDT_Pump = 9,
        CCDT_DRAM = 10,
        CCDT_WaterBlock = 11,
    }

    public enum CorsairAccessLevel
    {
        CAL_Shared = 0,
        CAL_ExclusiveLightingControl = 1,
        CAL_ExclusiveKeyEventsListening = 2,
        CAL_ExclusiveLightingControlAndKeyEventsListening = 3,
    }

    public partial struct CorsairVersion
    {
        public int major;

        public int minor;

        public int patch;
    }

    public partial struct CorsairSessionDetails
    {
        public CorsairVersion clientVersion;

        public CorsairVersion serverVersion;

        public CorsairVersion serverHostVersion;
    }

    public partial struct CorsairSessionStateChanged
    {
        public CorsairSessionState state;

        public CorsairSessionDetails details;
    }

    public unsafe partial struct CorsairDeviceInfo
    {
        public CorsairDeviceType type;

        [NativeTypeName("CorsairDeviceId")]
        public fixed sbyte id[128];

        [NativeTypeName("char[128]")]
        public fixed sbyte serial[128];

        [NativeTypeName("char[128]")]
        public fixed sbyte model[128];

        public int ledCount;

        public int channelCount;
    }

    public partial struct CorsairLedPosition
    {
        [NativeTypeName("CorsairLedLuid")]
        public uint id;

        public double cx;

        public double cy;
    }

    public partial struct CorsairDeviceFilter
    {
        public int deviceTypeMask;
    }

    public unsafe partial struct CorsairDeviceConnectionStatusChangedEvent
    {
        [NativeTypeName("CorsairDeviceId")]
        public fixed sbyte deviceId[128];

        [NativeTypeName("bool")]
        public byte isConnected;
    }

    public unsafe partial struct CorsairKeyEvent
    {
        [NativeTypeName("CorsairDeviceId")]
        public fixed sbyte deviceId[128];

        public CorsairMacroKeyId keyId;

        [NativeTypeName("bool")]
        public byte isPressed;
    }

    public unsafe partial struct CorsairEvent
    {
        public CorsairEventId id;

        [NativeTypeName("__AnonymousRecord_iCUESDK_L265_C2")]
        public _Anonymous_e__Union Anonymous;

        public ref CorsairDeviceConnectionStatusChangedEvent* deviceConnectionStatusChangedEvent
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref this, 1)).Anonymous.deviceConnectionStatusChangedEvent;
            }
        }

        public ref CorsairKeyEvent* keyEvent
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref this, 1)).Anonymous.keyEvent;
            }
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

    public unsafe partial struct CorsairDataType_BooleanArray
    {
        public bool* items;

        [NativeTypeName("unsigned int")]
        public uint count;
    }

    public unsafe partial struct CorsairDataType_Int32Array
    {
        public int* items;

        [NativeTypeName("unsigned int")]
        public uint count;
    }

    public unsafe partial struct CorsairDataType_Float64Array
    {
        public double* items;

        [NativeTypeName("unsigned int")]
        public uint count;
    }

    public unsafe partial struct CorsairDataType_StringArray
    {
        [NativeTypeName("char **")]
        public sbyte** items;

        [NativeTypeName("unsigned int")]
        public uint count;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct CorsairDataValue
    {
        [FieldOffset(0)]
        [NativeTypeName("bool")]
        public byte boolean;

        [FieldOffset(0)]
        public int int32;

        [FieldOffset(0)]
        public double float64;

        [FieldOffset(0)]
        [NativeTypeName("char *")]
        public sbyte* @string;

        [FieldOffset(0)]
        public CorsairDataType_BooleanArray boolean_array;

        [FieldOffset(0)]
        public CorsairDataType_Int32Array int32_array;

        [FieldOffset(0)]
        public CorsairDataType_Float64Array float64_array;

        [FieldOffset(0)]
        public CorsairDataType_StringArray string_array;
    }

    public partial struct CorsairProperty
    {
        public CorsairDataType type;

        public CorsairDataValue value;
    }

    public partial struct CorsairLedColor
    {
        [NativeTypeName("CorsairLedLuid")]
        public uint id;

        [NativeTypeName("unsigned char")]
        public byte r;

        [NativeTypeName("unsigned char")]
        public byte g;

        [NativeTypeName("unsigned char")]
        public byte b;

        [NativeTypeName("unsigned char")]
        public byte a;
    }

    public partial struct CorsairKeyEventConfiguration
    {
        public CorsairMacroKeyId keyId;

        [NativeTypeName("bool")]
        public byte isIntercepted;
    }

    [SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible")]
    [SuppressMessage("Interoperability", "SYSLIB1054:Use \'LibraryImportAttribute\' instead of \'DllImportAttribute\' to generate P/Invoke marshalling code at compile time")]
    public static unsafe partial class Methods
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

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairConnect([NativeTypeName("CorsairSessionStateChangedHandler")] delegate* unmanaged[Cdecl]<void*, CorsairSessionStateChanged*, void> onStateChanged, void* context);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairGetSessionDetails(CorsairSessionDetails* details);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairDisconnect();

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairGetDevices([NativeTypeName("const CorsairDeviceFilter *")] CorsairDeviceFilter* filter, int sizeMax, CorsairDeviceInfo* devices, int* size);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairGetDeviceInfo([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairDeviceInfo* deviceInfo);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairGetLedPositions([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, int sizeMax, CorsairLedPosition* ledPositions, int* size);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairSubscribeForEvents([NativeTypeName("CorsairEventHandler")] delegate* unmanaged[Cdecl]<void*, CorsairEvent*, void> onEvent, void* context);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairUnsubscribeFromEvents();

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairConfigureKeyEvent([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, [NativeTypeName("const CorsairKeyEventConfiguration *")] CorsairKeyEventConfiguration* config);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairGetDevicePropertyInfo([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairDevicePropertyId propertyId, [NativeTypeName("unsigned int")] uint index, CorsairDataType* dataType, [NativeTypeName("unsigned int *")] uint* flags);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairReadDeviceProperty([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairDevicePropertyId propertyId, [NativeTypeName("unsigned int")] uint index, CorsairProperty* property);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairWriteDeviceProperty([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairDevicePropertyId propertyId, [NativeTypeName("unsigned int")] uint index, [NativeTypeName("const CorsairProperty *")] CorsairProperty* property);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairFreeProperty(CorsairProperty* property);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairSetLedColors([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, int size, [NativeTypeName("const CorsairLedColor *")] CorsairLedColor* ledColors);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairSetLedColorsBuffer([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, int size, [NativeTypeName("const CorsairLedColor *")] CorsairLedColor* ledColors);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairSetLedColorsFlushBufferAsync([NativeTypeName("CorsairAsyncCallback")] delegate* unmanaged[Cdecl]<void*, CorsairError, void> callback, void* context);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairGetLedColors([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, int size, CorsairLedColor* ledColors);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairSetLayerPriority([NativeTypeName("unsigned int")] uint priority);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairGetLedLuidForKeyName([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, [NativeTypeName("char")] sbyte keyName, [NativeTypeName("CorsairLedLuid *")] uint* ledId);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairRequestControl([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId, CorsairAccessLevel accessLevel);

        [DllImport(ArchitectureConstants.iCUESDK, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CorsairError CorsairReleaseControl([NativeTypeName("const CorsairDeviceId")] sbyte* deviceId);
    }
}
