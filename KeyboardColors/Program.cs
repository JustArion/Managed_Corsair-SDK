
/*
    Requirements:
        'iCUE.exe' is running.
        You have a Corsair Keyboard
*/

using Dawn.CorsairSDK;
using Dawn.CorsairSDK.Extensions;
using Dawn.CorsairSDK.LowLevel;

var device = CorsairSDK.GetDevices(CorsairDeviceType.CDT_Keyboard).FirstOrDefault();

if (device.type == CorsairDeviceType.CDT_Unknown)
{
    Console.WriteLine("[!] Could not find any Corsair keyboards connected.");
    Environment.Exit(1);
}

var ledController = device.GetLedController();

if (ledController.LedCount == 0)
{
    Console.WriteLine($"[!] They keyboard '{device.GetModel()}' doesn't seem to have any adjustable LEDs.");
    Environment.Exit(2);
}

var (success, ledInformation) = ledController.TryGetLedInformation();

if (!success)
{
    Console.WriteLine($"[!] Unable to get Led Information for '{device.GetModel()}'");
    Environment.Exit(3);
}

using (ledController.RequestControl())
{
    foreach (var (position, color) in ledInformation.OrderBy(x => x.Position.cy))
    {
        ledController.SetLedColor(position, color);
        await Task.Delay(50);
        ledController.SetLedColor(position, (0, 0, 0, 255));

    }

    await Task.Delay(TimeSpan.FromSeconds(1));
}
