using Dawn.CorsairSDK;
using Dawn.CorsairSDK.Extensions;
using Dawn.CorsairSDK.LowLevel;
using WelcomeBackExplosion;
using Timer = System.Timers.Timer;

/// Configuration
var inactiveThreshold = TimeSpan.FromSeconds(10);
///


Helpers.CheckDeviceConditions();
#if DEBUG
await HandleExplosion();
#else
bool? explosionReady = null;

var smp = new SemaphoreSlim(1, 1); // Only 1 explosion at a time please.
var timer = new Timer();
timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
timer.Elapsed += async delegate
{
    await smp.WaitAsync();
    try
    {
        if (explosionReady.HasValue && explosionReady.Value)
        {
            Console.WriteLine();
            Console.WriteLine("Explosion mark!");
            explosionReady = null;
            await HandleExplosion(); // Detonate the explosive.
            Console.WriteLine("I am a teacu--");
            Environment.Exit(418);
        }
        else
        {
            var inactiveTime = Helpers.InactiveFor();

        
            if (explosionReady.HasValue)
            {
                Console.Write("\r" + new string(' ', $"\rYou are inactive for: {inactiveThreshold.Seconds} / {inactiveThreshold.Seconds}".Length)); // This clears that line.
                Console.Write("\rExplosive Primed");
            }
            else
            {
                Console.Write($"\rYou are inactive for: {inactiveTime.Seconds} / {inactiveThreshold.Seconds}");
            }
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);


            if (inactiveTime >= inactiveThreshold) 
                explosionReady = false; // Prime the explosive.

            if (inactiveTime.Seconds == 0 && explosionReady.HasValue && !explosionReady.Value)
                explosionReady = true; // Arm the explosive.
        }
    }
    finally
    {
        smp.Release();
    }
};
timer.Start();

await Task.Delay(-1);
#endif
return;



async Task HandleExplosion()
{
    const int DELAY_MS = 75;
    const int EXPLOSION_RADIUS = 150;
    
    var keyboard = CorsairSDK.GetDevices(CorsairDeviceType.CDT_Keyboard).First();

    var ledController = keyboard.GetLedController();

    var ledInformation = ledController.GetLedInformation();
    
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

                var color = Colors.RandomColor(coord.id);
                ledController.TrySetLedColor(color);
            }

            currentRadius += 1;
            await Task.Delay(DELAY_MS);
        }
    }
}

(double X, double Y) CalculateCenter(IReadOnlyCollection<CorsairLedPosition> positions)
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

