using System.Text;
using Corsair.Bindings;
using Corsair.Device;

namespace Corsair;

public static class CorsairInformation
{
    public static void PrintDeviceInformation()
    {
        foreach (var deviceInfo in CorsairSDK.GetDevices())
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Corsair {deviceInfo.Model} [{deviceInfo.Type}]");

            foreach (var supportedProperty in deviceInfo.SupportedFeatures)
            {

                var propVal = deviceInfo._interop.ReadDeviceProperty(deviceInfo.Id, supportedProperty);

                var cleanedVal = propVal.Value;

                if (deviceInfo.Type == DeviceType.Keyboard)
                    AddLayout(supportedProperty, ref cleanedVal);

                if (cleanedVal is Array arr)
                    cleanedVal = $"[ {string.Join(", ", arr.Cast<object>())} ]";

                sb.AppendLine($"\t[{(int)supportedProperty}][{PresentPropertyFlags(deviceInfo, supportedProperty)}] {supportedProperty}: {cleanedVal}");
            }

            var hasChannels = deviceInfo.ChannelCount > 0;
            var hasLeds = deviceInfo.LedCount > 0;

            if (hasChannels || hasLeds)
            {
                sb.AppendLine("\tExtra Info:");

                if (hasChannels)
                    sb.AppendLine($"\tChannels: {deviceInfo.ChannelCount}");

                if (hasLeds)
                    sb.AppendLine($"\tLeds: {deviceInfo.LedCount}");
            }



            Console.WriteLine(sb.ToString());
        }
    }

    private static void AddLayout(DeviceProperty supportedProperty, ref object cleanedVal)
    {
        if (cleanedVal is not int)
            return;

        cleanedVal = supportedProperty switch {
            DeviceProperty.LogicalLayout => (LogicalLayout)cleanedVal,
            DeviceProperty.PhysicalLayout => (PhysicalLayout)cleanedVal,
            _ => cleanedVal
        };
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
