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

using (ledController.RequestControl(CorsairAccessLevel.CAL_ExclusiveLightingControl))
{
    var info = ledInformation.OrderBy(x => x.Position.cy).ToArray();
    for (var i = 0; i < info.Length; i++)
    {
        var position = info[i].Position;
        var color = info[i].Color;
        var inverseLength = info.Length -1 - i;
        var inversePosition = info[inverseLength].Position;
        var inverseColor = info[inverseLength].Color;
        
        await ledController.SetLedColorsAsync(
            color with { id = position.id }, 
            inverseColor with { id = inversePosition.id });
        
        await Task.Delay(50);
        
        await ledController.SetLedColorsAsync(
            (position.id, LedController.LedOffColor),
            (inversePosition.id, LedController.LedOffColor));
    }
    
    await Task.Delay(TimeSpan.FromSeconds(1));
}