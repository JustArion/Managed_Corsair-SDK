namespace Tests;

using System.Drawing;
using Dawn.CorsairSDK.Rewrite;
using Dawn.Rewrite;

public class GlobalDisposesProperly
{
    public GlobalDisposesProperly()
    {
        Task.Run(async () =>
        {
            var colors = CorsairSDK.KeyboardLighting.Colors;
            IDisposable disposable;

            Console.WriteLine("Setting Global Red");
            using (colors.SetGlobal(Color.Red))
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                Console.WriteLine("Setting Global Green");
                disposable = colors.SetGlobal(Color.Green);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            Console.WriteLine("Disposing Red");
            await Task.Delay(TimeSpan.FromSeconds(2));

            Console.WriteLine("Clearing Page Keys");
            colors.ClearZones(KeyboardZones.PageKeys);

            await Task.Delay(TimeSpan.FromSeconds(3));
            Console.WriteLine("Clearing green");
            disposable.Dispose();
        }).Wait();
    }
}