using System.Diagnostics;
using System.Drawing;
using System.Reactive.Disposables;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Internal;

internal partial class EffectController
{
    // public record PulseInfo(Color Start, Color End, TimeSpan Interval, bool IsInfinite, TimeSpan TotalDuration = default)
    public EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKey[] keys) => PulseKeys(pulseInfo, (IEnumerable<KeyboardKey>)keys);

    public EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys)
    {
        colorController.ThrowIfDisconnected();
        var keysArray = keys.ToArray();

        if (IsCircular(pulseInfo))
        {
            var circularDisposable = colorController.SetKeys(pulseInfo.Start, keysArray);

            if (pulseInfo.IsInfinite)
                return new EffectReceipt(Task.CompletedTask, circularDisposable);

            // Set up a callback to cancel the set keys after the duration of the animation
            var circularCts = new CancellationTokenSource();
            circularCts.Token.Register(x => ((IDisposable)x!).Dispose(), circularDisposable);
            circularCts.CancelAfter(pulseInfo.TotalDuration);

            return new EffectReceipt(Task.CompletedTask, circularDisposable);
        }

        // Debug.WriteLine(pulseInfo.ToString(), $"{nameof(PulseKeys)}({nameof(PulseInfo)} {nameof(pulseInfo)}, {nameof(KeyboardKeys)}[] {nameof(keys)})");

        var disposables = new List<IDisposable?>();
        var cts = new CancellationTokenSource();
        var controlledKeys = new List<KeyboardKey>(keysArray);


        var effectKeysReceipt = _receiptHandler.Set(keysArray, keyboardKey => Disposable.Create(keyboardKey, key => {
            lock (controlledKeys)
            {
                controlledKeys.Remove(key);
                colorController.ClearKeys(key);
                Debug.WriteLine($"Clearing Key: {key}", $"{nameof(PulseKeys)}({nameof(PulseInfo)} {nameof(pulseInfo)}, {nameof(KeyboardKey)}[] {nameof(keys)})");

                if (controlledKeys.Count == 0)
                    cts.Cancel();
            }
        }));

        disposables.Add(effectKeysReceipt);
        disposables.Add(Disposable.Create(cts, source => {
            if (!source.IsCancellationRequested)
                source.Cancel();
        }));

        // ReSharper disable once MethodSupportsCancellation
        var pulseTask = Task.Run(()=> DoKeyPulses(pulseInfo, controlledKeys, cts.Token));

        var disposable = Disposable.Create(disposables, list => {
            foreach (var disposable in list)
                disposable?.Dispose();
        });
        return new EffectReceipt(pulseTask, disposable);
    }

    // We save some compute here, we don't need to animate something thats not moving.
    /// <summary>
    /// The Start color is the end color
    /// </summary>
    private bool IsCircular(PulseInfo pulseInfo) => pulseInfo.Start == pulseInfo.End;

    public EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKeyState[] keys)
        => PulseKeys(pulseInfo, keys.Select(x => x.Key));

    private async Task DoKeyPulses(PulseInfo pulseInfo, List<KeyboardKey> controlledKeys, CancellationToken token)
    {
        var startTime = Environment.TickCount;
        var intervalMs = pulseInfo.Interval.TotalMilliseconds;

        while (!token.IsCancellationRequested && PulseIsOccurring(pulseInfo, startTime))
        {
            var deltaTime = Environment.TickCount - startTime;

            var period = Math.Clamp(intervalMs, 1, double.MaxValue); // Prevent Div by 0

            var progress = (float)(deltaTime / period);

            // X: Time | Y : Color
            var wave = CalculateWave(pulseInfo, progress);

            // Wave
            // Backup Wave
            // Progression
            var t = InspectWave(wave,
                () => InspectWave((float)(pulseInfo.OnNan?.Invoke(progress) ?? progress),
                    () => progress));


            var lerpedColor = pulseInfo.UseSmoothPulses
                ? ColorUtility.SlerpColor(pulseInfo.Start, pulseInfo.End, t)
                : ColorUtility.LerpColor(pulseInfo.Start, pulseInfo.End, t);


            #if DEBUG
            if (Math.Round(progress, 2) % 0.5 == 0)
            {
                string color;
                if (lerpedColor is { R: 255, G: 0 })
                    color = "Red";
                else if (lerpedColor.G == Color.Green.G)
                    color = "Green";
                else color = $"R:{lerpedColor.R} G: {lerpedColor.G}";

                // X : Time     Y: Color
                var x = Math.Round(progress, 2);
                var y = Math.Round(t, 2);
                Trace.WriteLine($"({x:F}, {y:F})\t| {deltaTime / 1000}s \t| {color}");
            }
            #endif


            colorController.SetKeys(lerpedColor, controlledKeys);
            await Task.Delay(1, token);
        }
    }

    private static float InspectWave(float initial, Func<float> onNan) => float.IsNaN(initial) ? onNan() : initial;

    private static float CalculateWave(PulseInfo info, float x)
        => info.WaveModulation == null ? (float)CommonWaveModulations.Sine(x) : (float)info.WaveModulation(x);

    private static bool PulseIsOccurring(PulseInfo pulseInfo, int startTime)
    {
        if (pulseInfo.IsInfinite) // Infinity
            return true;

        var durationPassed = Environment.TickCount - startTime;

        return durationPassed < pulseInfo.TotalDuration.TotalMilliseconds; // Duration hasn't been reached yet
    }


    public EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeyState> keys)
        => PulseKeys(pulseInfo, keys.Select(x => x.Key));

    public EffectReceipt PulseZones(PulseInfo pulseInfo, KeyboardZones zones)
    {
        colorController.ThrowIfDisconnected();


        var keys = ZoneUtility.GetKeysFromZones(zones, _device);

        return PulseKeys(pulseInfo, keys);
    }
}
