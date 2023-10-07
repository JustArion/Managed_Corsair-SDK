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
    var info = await PlayRacingAnimation(ledInformation, ledController);

    await Task.Delay(TimeSpan.FromSeconds(1));
    
    var groupedColors = await PlayNightRiderAnimation(info, ledController);

    await Task.Delay(TimeSpan.FromSeconds(1));
    
    await PlayNightRiderPureRGBAnimation(groupedColors, ledController);

    await Task.Delay(TimeSpan.FromSeconds(1));
    
    await PlayBreathingAnimation(info, ledController);
}

return;

async Task<LedInformation[]> PlayRacingAnimation(IEnumerable<LedInformation> ledInformations, LedController ledController1)
{
    var info1 = ledInformations.OrderBy(x => x.Position.cx).ToArray();
    for (var i = 0; i < info1.Length; i++)
    {
        var position = info1[i].Position;
        var color = info1[i].Color;
        var inverseLength = info1.Length - 1 - i;
        var inversePosition = info1[inverseLength].Position;
        var inverseColor = info1[inverseLength].Color;

        await ledController1.SetLedColorsAsync(
            color with { id = position.id },
            inverseColor with { id = inversePosition.id });

        await Task.Delay(50);

        await ledController1.SetLedColorsAsync(
            (position.id, LedController.LedOffColor),
            (inversePosition.id, LedController.LedOffColor));
    }

    return info1;
}

async Task<IGrouping<double, LedInformation>[]> PlayNightRiderAnimation(IEnumerable<LedInformation> ledInformations1, LedController ledController2)
{
    var groupings = ledInformations1.GroupBy(x => x.Position.cx).ToArray();
    Array.Reverse(groupings); // Reversing the array ensures that we start from the right hand side.
    for (var i = 0; i < 5; i++)
    {
        foreach (var currentGrouping in groupings)
        {
            var colorComponentValue = i % 3 == 0 ? 255 : 0;
            //
            var modifiedColors = currentGrouping.Select(x => x.Color with
            {
                r = (byte)(i % 3 == 0 ? colorComponentValue : x.Color.r),
                g = (byte)(i % 3 == 1 ? colorComponentValue : x.Color.g),
                b = (byte)(i % 3 == 2 ? colorComponentValue : x.Color.b)
            }).ToArray();

            await ledController2.SetLedColorsAsync(modifiedColors);

            await Task.Delay(25);
        }

        Array.Reverse(groupings); // This causes that bounce back effect, when the line is finished, it slides back.
    }

    return groupings;
}

async Task PlayNightRiderPureRGBAnimation(IGrouping<double, LedInformation>[] groupedColors1, LedController ledController3)
{
    Array.Reverse(groupedColors1); // Reversing the array ensures that we start from the right hand side.
    for (var i = 0; i < 5; i++)
    {
        foreach (var currentGrouping in groupedColors1)
        {
            var modifiedColors = currentGrouping.Select(x => x.Color with
            {
                r = (byte)(i % 3 == 0 ? 255 : 0),
                g = (byte)(i % 3 == 1 ? 255 : 0),
                b = (byte)(i % 3 == 2 ? 255 : 0)
            }).ToArray();

            await ledController3.SetLedColorsAsync(modifiedColors);

            await Task.Delay(25);
        }

        Array.Reverse(
            groupedColors1); // This causes that bounce back effect, when the line is finished, it slides back.
    }
}

async Task PlayBreathingAnimation(LedInformation[] info2, LedController ledController4)
{
    // Breathe Effect
    for (var i = 0; i < 3; i++)
    {
        var currentColors = info2.Select(x => x.Color).ToArray();

        // Original -> Off
        await ledController4.Gradient(currentColors, LedController.ToOffLedColors(currentColors),
            TimeSpan.FromSeconds(3));

        // Off -> Original
        await ledController4.Gradient(LedController.ToOffLedColors(currentColors), currentColors,
            TimeSpan.FromSeconds(3));
    }
}