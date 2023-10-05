#if DEBUG
namespace Dawn.Libs.Corsair.SDK;

using System.Text;
using Extensions;
using LowLevel;

internal static class InteropInformation
{
    internal static void DumpDeviceInformation()
    {
        foreach (var deviceInfo in CorsairSDK.GetDevices())
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Corsair {deviceInfo.GetModel()}");

            foreach (var supportedProperty in deviceInfo.GetSupportedProperties())
            {

                var propVal = deviceInfo.ReadDeviceProperty(supportedProperty);

                var cleanedVal = propVal.Value;
                if (cleanedVal is Array arr)
                {
                    cleanedVal = $"[ {string.Join(", ", arr.Cast<object>())} ]";
                }
            
                sb.AppendLine($"\t[{(int)supportedProperty}][{PresentPropertyFlags(deviceInfo, supportedProperty)}] {supportedProperty}: {cleanedVal}");
            }

            Console.WriteLine(sb.ToString());
        }
    }

    private static string PresentPropertyFlags(CorsairDeviceInfo info, CorsairDevicePropertyId supportedProperty)
    {
        var (_, flags) = info.GetPropertyInfo(supportedProperty);

        switch (flags)
        {
            case CorsairPropertyFlag.CPF_None:
                return "N/A";
            case CorsairPropertyFlag.CPF_CanRead:
                return "ReadOnly";
            case CorsairPropertyFlag.CPF_CanWrite:
                return "WriteOnly";
            case CorsairPropertyFlag.CPF_Indexed:
                return "Read/Write";
            default:
                throw new ArgumentOutOfRangeException(nameof(info));
        }
    }
}
#endif