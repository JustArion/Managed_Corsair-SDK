﻿
/*
    Requirements:
        'iCUE.exe' is running.
        You have a Wireless Corsair Headset
*/

using Dawn.CorsairSDK;
using Dawn.CorsairSDK.Extensions;
using Dawn.CorsairSDK.LowLevel;

var device = CorsairSDK.GetDevices(CorsairDeviceType.CDT_Headset).FirstOrDefault(x => x.HasSupportedProperty(CorsairDevicePropertyId.CDPI_BatteryLevel));

if (device.type == CorsairDeviceType.CDT_Unknown)
{
    Console.WriteLine("[!] Could not find any Corsair headsets connected, your headset might be off.");
    Environment.Exit(1);
}

var (success, batteryLevel) = device.TryGetBatteryLevel();

if (!success)
{
    Console.WriteLine($"[!] Unable to get battery level for '{device.GetModel()}'.");
    Environment.Exit(2);
}

Console.WriteLine($"[*] The battery level for '{device.GetModel()}' is {batteryLevel}%");
