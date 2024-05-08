
/*
    Requirements:
        'iCUE.exe' is running.
        You have a Wireless Corsair Headset
*/

using Corsair;
using Corsair.Device;
using Corsair.Device.Devices;


var device = CorsairSDK.GetDevices(DeviceType.Headset).FirstOrDefault(x => x.HasFeature(DeviceProperty.BatteryLevel));

if (device == null)
{
    Console.WriteLine("[!] Could not find any Corsair headsets connected, your headset might be off.");
    Environment.Exit(1);
}

var headset = device.AsDevice<Headset>();
var batteryLevel = headset.BatteryLevel;


Console.WriteLine($"[*] The battery level for '{device.Model}' is {batteryLevel}%");
