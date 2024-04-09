namespace Dawn.CorsairSDK;

using Extensions;
using LowLevel;

public static class Effects
{
    public static async ValueTask PlayEffect_NightRider(this LedController controller)
    {
        var (success, ledInformation) = TryGetEffectController(controller);
        if (!success)
            return;

        await controller._NightRider(ledInformation);
    }

    internal static async Task _NightRider(this LedController controller, IEnumerable<LedInformation> ledInformation)
    {
        var groupings = ledInformation.GroupBy(x => x.Position.cx).ToArray();
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

                await controller.SetLedColorsAsync(modifiedColors);

                await Task.Delay(25);
            }

            Array.Reverse(groupings); // This causes that bounce back effect, when the line is finished, it slides back.
        }
    }


    public static async ValueTask PlayEffect_Racing(this LedController controller)
    {
        var (success, ledInformation) = TryGetEffectController(controller);
        if (!success)
            return;

        await controller._Racing(ledInformation);
    }

    internal static async Task _Racing(this LedController controller, IEnumerable<LedInformation> ledInformation)
    {
        const int DELAY_MS = 50;
        // 2 starting points, left and right.
        // Start point Left: Goes left -> right from top to bottom.
        // Start point right: Goet right -> left from bottom to top.
        var info1 = ledInformation.OrderBy(x => x.Position.cx).ToArray();
        for (var i = 0; i < info1.Length; i++)
        {
            var position = info1[i].Position;
            var color = info1[i].Color;
            var inverseLength = info1.Length - 1 - i;
            var inversePosition = info1[inverseLength].Position;
            var inverseColor = info1[inverseLength].Color;

            await controller.SetLedColorsAsync(
                color with { id = position.id },
                inverseColor with { id = inversePosition.id });

            await Task.Delay(DELAY_MS);

            await controller.SetLedColorsAsync(
                (position.id, LedController.LedOffColor),
                (inversePosition.id, LedController.LedOffColor));
        }
    }

    public static async ValueTask PlayEffect_NightRiderPureRGB(this LedController controller)
    {
        var (success, ledInformation) = TryGetEffectController(controller);
        if (!success)
            return;

        await controller._NightRiderPureRGB(ledInformation);
    }
    
    internal static async Task _NightRiderPureRGB(this LedController controller, IEnumerable<LedInformation> ledInformation)
    {
        const int DELAY_MS = 25;
        
        var groupings = ledInformation.GroupBy(x => x.Position.cx).ToArray();
        Array.Reverse(groupings); // Reversing the array ensures that we start from the right hand side.
        for (var i = 0; i < 5; i++)
        {
            foreach (var currentGrouping in groupings)
            {
                var modifiedColors = currentGrouping.Select(x => x.Color with
                {
                    r = (byte)(i % 3 == 0 ? 255 : 0),
                    g = (byte)(i % 3 == 1 ? 255 : 0),
                    b = (byte)(i % 3 == 2 ? 255 : 0)
                }).ToArray();

                await controller.SetLedColorsAsync(modifiedColors);

                await Task.Delay(DELAY_MS);
            }

            Array.Reverse(groupings); // This causes that bounce back effect, when the line is finished, it slides back.
        }
    }

    public static async ValueTask PlayEffect_Breathing(this LedController controller)
    {
        var (success, ledInformation) = TryGetEffectController(controller);
        if (!success)
            return;

        await controller._Breathing(ledInformation);
    }
    
    internal static async Task _Breathing(this LedController controller, LedInformation[] ledInformation)
    {
        const int DELAY_SEC = 3;
        
        // Breathe Effect
        for (var i = 0; i < 3; i++)
        {
            var currentColors = ledInformation.Select(x => x.Color).ToArray();

            // Original -> Off
            await controller.Gradient(currentColors, LedController.ToOffLedColors(currentColors),
                TimeSpan.FromSeconds(DELAY_SEC));

            // Off -> Original
            await controller.Gradient(LedController.ToOffLedColors(currentColors), currentColors,
                TimeSpan.FromSeconds(DELAY_SEC));
        }
    }
    
    public static async ValueTask PlayEffect_Explosion(this LedController controller)
    {
        var (success, ledInformation) = TryGetEffectController(controller);
        if (!success)
            return;

        await controller._Explosion(ledInformation);
    }
    
    internal static async Task _Explosion(this LedController controller, IEnumerable<LedInformation> ledInformation)
    {
        const int DELAY_MS = 15;
        const int EXPLOSION_RADIUS = 200;
    
        var keyboard = CorsairSDK.GetDevices(CorsairDeviceType.CDT_Keyboard).First();

        var ledController = keyboard.GetLedController();
    
        using (ledController.RequestControl())
        {
            var coordinates = ledInformation.OrderByDescending(x => x.Position.cy).Select(x => x.Position).ToArray();

            var midpoint = CalculateCenter(coordinates);
            double currentRadius = 0;

            while (currentRadius < EXPLOSION_RADIUS)
            {
                foreach (var coord in coordinates)
                {
                    var coordX = coord.cx;
                    var coordY = coord.cy;

                    var distance = Math.Sqrt(Math.Pow(coordX - midpoint.X, 2) + Math.Pow(coordY - midpoint.Y, 2));
                    if (!(distance <= currentRadius)) 
                        continue;
                
                    var color = Colors.RandomColor(coord.id, 255);
                    ledController.TrySetLedColor(color);
                }

                currentRadius += 1;
                await Task.Delay(DELAY_MS);
            }
        }
    }
    
    private static (double X, double Y) CalculateCenter(IReadOnlyCollection<CorsairLedPosition> positions)
    {
        double sumX = 0, sumY = 0;
        var count = positions.Count;
    
        foreach (var position in positions)
        {
            sumX += position.cx;
            sumY += position.cy;
        }

        return (sumX / count, sumY / count);
    }
    

    private static (bool Success, LedInformation[] LedInformation) TryGetEffectController(LedController controller) =>
        controller.LedCount == 0 
            ? (false, Array.Empty<LedInformation>()) 
            : controller.TryGetLedInformation();

    private static async Task RunOnAllDevices(Func<LedController, ValueTask> enumerable)
    {
        var validDevices = CorsairSDK.GetDevices().Where(x => x.ledCount > 0);

        var tasks = validDevices.Select(device =>
        {
            var controller = device.GetLedController();
            return Task.Run(async () => await enumerable(controller));
        }).ToList();

        await Task.WhenAll(tasks);
    }
    
    public static async Task PlayEffect_NightRider() => 
        await RunOnAllDevices(async controller => await controller.PlayEffect_NightRider());

    public static async Task PlayEffect_Racing() => 
        await RunOnAllDevices(async controller => await controller.PlayEffect_Racing());
    public static async Task PlayEffect_Breathing() => 
        await RunOnAllDevices(async controller => await controller.PlayEffect_Breathing());
    public static async Task PlayEffect_Explosion() => 
        await RunOnAllDevices(async controller => await controller.PlayEffect_Explosion());
}