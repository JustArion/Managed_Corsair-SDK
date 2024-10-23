#if DEBUG
// #define IMMEDIATE_EXPLOSION
#endif
using Corsair;
using Corsair.Device;
using Corsair.Device.Devices;
using Corsair.Lighting;
using WelcomeBackExplosion;
using Timer = System.Timers.Timer;

/// Configuration
var inactiveThreshold = TimeSpan.FromSeconds(10);
///


Helpers.CheckDeviceConditions();
#if IMMEDIATE_EXPLOSION
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

    var keyboard = CorsairSDK.GetDevices(DeviceType.Keyboard).First().AsDevice<Keyboard>();
    var lighting = keyboard.KeyboardLighting;
    var colors = lighting.Colors;

    lighting.TryInitialize();

    var keys = colors.KeyboardKeys.OrderByDescending(x => x.Coordinate.Y).ToArray();

    var midpoint = CalculateCenter(keys);
    
    double currentRadius = 0;

    while (currentRadius < EXPLOSION_RADIUS)
    {
        foreach (var (key, coordinate) in keys)
        {
            var coordX = coordinate.X;
            var coordY = coordinate.Y;

            var distance = Math.Sqrt(Math.Pow(coordX - midpoint.X, 2) + Math.Pow(coordY - midpoint.Y, 2));
            if (!(distance <= currentRadius)) 
                continue;

            var color = ColorUtility.RandomColor();

            colors.SetKeys(color, key);
        }

        currentRadius += 1;
        await Task.Delay(DELAY_MS);
    }
}

(double X, double Y) CalculateCenter(KeyboardKeyState[] positions)
{
    double sumX = 0, sumY = 0;
    var count = positions.Length;
    
    foreach (var (_, position) in positions)
    {
        sumX += position.X;
        sumY += position.Y;
    }

    return (sumX / count, sumY / count);
}

