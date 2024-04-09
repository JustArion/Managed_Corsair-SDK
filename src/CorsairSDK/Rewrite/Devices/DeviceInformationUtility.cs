using System.Text;
using Dawn.CorsairSDK.Bindings;
using Dawn.CorsairSDK.Extensions;

namespace Dawn.CorsairSDK.Rewrite.Device;

public static class DeviceInformationUtility
{
    public static void PrintDeviceInformation()
    {
        Dawn.Rewrite.CorsairSDK._connectionHandler.Connect();
        foreach (var deviceInfo in Dawn.Rewrite.CorsairSDK.GetDevices())
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Corsair {deviceInfo.Model} [{deviceInfo.Type}]");

            foreach (var supportedProperty in deviceInfo.SupportedFeatures)
            {

                var propVal = deviceInfo._interop.ReadDeviceProperty(deviceInfo.Id, supportedProperty);

                var cleanedVal = propVal.Value;
                if (cleanedVal is Array arr)
                    cleanedVal = $"[ {string.Join(", ", arr.Cast<object>())} ]";

                sb.AppendLine($"\t[{(int)supportedProperty}][{PresentPropertyFlags(deviceInfo, supportedProperty)}] {supportedProperty}: {cleanedVal}");
            }

            Console.WriteLine(sb.ToString());
        }
    }

    private static string PresentPropertyFlags(CorsairDevice info, DeviceProperty supportedProperty)
    {
        var (_, flags) = info._interop.GetPropertyInfo(info.Id, supportedProperty);

        return flags switch
        {
            CorsairPropertyFlag.CPF_None => "N/A",
            CorsairPropertyFlag.CPF_CanRead => "ReadOnly",
            CorsairPropertyFlag.CPF_CanWrite => "WriteOnly",
            CorsairPropertyFlag.CPF_Indexed => "Read/Write",
            _ => throw new ArgumentOutOfRangeException(nameof(info))
        };
    }
}
