
/*
    Requirements:
        'iCUE.exe' is running.
        You have a Corsair Keyboard
*/

using System.Drawing;
using Corsair;
using Corsair.Device;
using Corsair.Device.Devices;
using Corsair.Lighting;

var device = CorsairSDK.GetDevices(DeviceType.Keyboard).FirstOrDefault();

if (device == null)
{
    Console.WriteLine("[!] Could not find any Corsair keyboards connected.");
    Environment.Exit(1);
}

var keyboard = device.AsDevice<Keyboard>();


if (keyboard.LedCount == 0)
{
    Console.WriteLine($"[!] They keyboard '{device.Model}' doesn't seem to have any adjustable LEDs.");
    Environment.Exit(2);
}

keyboard.KeyboardLighting.TryInitialize(AccessLevel.Shared);

Console.WriteLine($"[*] Keyboard Colors on 'Corsair {device.Model}'");

using (var controller = keyboard.KeyboardLighting.Colors)
{
    // Horizontal From the Top Left
    foreach (var keyboardKey in controller.KeyboardKeys.OrderBy(x => x.Coordinate.Y))
    {
        controller.SetKeys(keyboardKey.Color, keyboardKey.Key);
        await Task.Delay(50);
        controller.SetKeys(Color.Black, keyboardKey.Key);
    }
    await Task.Delay(TimeSpan.FromSeconds(1));
}