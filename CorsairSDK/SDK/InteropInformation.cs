namespace Dawn.CorsairSDK;

using System.Text;
using Extensions;
using LowLevel;

public static class InteropInformation
{
    public static void DumpDeviceInformation()
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
                    cleanedVal = $"[ {string.Join(", ", arr.Cast<object>())} ]";

                sb.AppendLine($"\t[{(int)supportedProperty}][{PresentPropertyFlags(deviceInfo, supportedProperty)}] {supportedProperty}: {cleanedVal}");
            }

            Console.WriteLine(sb.ToString());
        }
    }

    private static string PresentPropertyFlags(CorsairDeviceInfo info, CorsairDevicePropertyId supportedProperty)
    {
        var (_, flags) = info.GetPropertyInfo(supportedProperty);

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