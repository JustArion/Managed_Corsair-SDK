﻿namespace WelcomeBackExplosion;

using Corsair;
using Corsair.Device;
using Vanara.PInvoke;

public static class Helpers
{
    
    private static readonly Lazy<DateTimeOffset> _SystemStartTime = new (()=> DateTimeOffset.UtcNow.AddMilliseconds(-Environment.TickCount64));
    
    public static TimeSpan InactiveFor()
    {
        var inputInfo = User32.LASTINPUTINFO.Default;
        if (!User32.GetLastInputInfo(ref inputInfo))
            return TimeSpan.FromSeconds(0);
        
        var lastInput = _SystemStartTime.Value.AddMilliseconds(inputInfo.dwTime);
        var now = DateTimeOffset.UtcNow;
    
        var delta = now - lastInput;

        return delta;
    }
    internal static void CheckDeviceConditions()
    {
        if (CorsairSDK.GetDevices(DeviceType.Keyboard).FirstOrDefault() != null)
            return;
    
        Console.WriteLine("[!] Your system doesn't have the necessary hardware to support this kind of sample");
        Environment.Exit(1);
    }
}