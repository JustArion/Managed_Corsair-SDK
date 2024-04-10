#define TRACE
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Disposables;
using Dawn.CorsairSDK.Rewrite.Lighting.Contracts;

namespace Dawn.CorsairSDK.Rewrite.Lighting.Internal;

internal class EffectController(KeyboardColorController colorController) : IEffectController
{
    public void Dispose() => _receiptHandler.DisposeAndClear();

    private readonly ReceiptHandler<KeyboardKeys> _receiptHandler = new();


    public EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys) => PulseKeys(pulseInfo, (IEnumerable<KeyboardKeys>)keys);

    public EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys)
    {
        colorController.ThrowIfDisconnected();
        var keysArray = keys.ToArray();

        // Debug.WriteLine(pulseInfo.ToString(), $"{nameof(PulseKeys)}({nameof(PulseInfo)} {nameof(pulseInfo)}, {nameof(KeyboardKeys)}[] {nameof(keys)})");

        var disposables = new List<IDisposable?>();
        var cts = new CancellationTokenSource();
        var controlledKeys = new List<KeyboardKeys>(keysArray);


        var controlledKeysReceipt = _receiptHandler.Set(keysArray, keyboardKey => Disposable.Create(keyboardKey, key => {
            lock (controlledKeys)
            {
                controlledKeys.Remove(key);
                colorController.ClearKeys(key);
                Debug.WriteLine($"Clearing Key: {key}", $"{nameof(PulseKeys)}({nameof(PulseInfo)} {nameof(pulseInfo)}, {nameof(KeyboardKeys)}[] {nameof(keys)})");

                if (controlledKeys.Count == 0)
                    cts.Cancel();
            }
        }));

        disposables.Add(controlledKeysReceipt);
        disposables.Add(Disposable.Create(cts, source => {
            if (!source.IsCancellationRequested)
                source.Cancel();
        }));

        // ReSharper disable once MethodSupportsCancellation
        var pulseTask = Task.Run(()=> DoPulseKeys(pulseInfo, controlledKeys, cts.Token));


        var disposable = Disposable.Create(disposables, list => {
            foreach (var disposable in list)
                disposable?.Dispose();
        });
        return new EffectReceipt(pulseTask, disposable);
    }

    public EffectReceipt PulseKeys(PulseInfo pulseInfo, params KeyboardKey[] keys)
        => PulseKeys(pulseInfo, keys.Select(x => x.Key));

    private async Task DoPulseKeys(PulseInfo pulseInfo, List<KeyboardKeys> controlledKeys, CancellationToken token)
    {
        var startTime = Environment.TickCount;
        var intervalMs = pulseInfo.Interval.TotalMilliseconds;

        while (!token.IsCancellationRequested && PulseIsOccurring(pulseInfo, startTime))
        {
            var deltaTime = Environment.TickCount - startTime;

            var period = Math.Clamp(intervalMs, 1, double.MaxValue);

            var progress = (float)(deltaTime / period);

            // X: Time | Y : Color
            var wave = CalculateWave(pulseInfo, progress);

            // Wave
            // Backup Wave
            // Progression
            var t = InspectWave(wave,
                () => InspectWave((float)(pulseInfo.OnNan?.Invoke(progress) ?? progress),
                    () => progress));



            var lerpedColor = ColorEffects.LerpColor(pulseInfo.Start, pulseInfo.End, t);


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

    private static float InspectWave(float initial, Func<float> backup) => float.IsNaN(initial) ? backup() : initial;

    private static float CalculateWave(PulseInfo info, float x)
        => info.WaveModulation == null ? x : (float)info.WaveModulation(x);

    private static bool PulseIsOccurring(PulseInfo pulseInfo, int startTime)
    {
        if (pulseInfo.IsInfinite) // Infinity
            return true;

        var durationPassed = Environment.TickCount - startTime;

        return durationPassed < pulseInfo.TotalDuration.TotalMilliseconds; // Duration hasn't been reached yet
    }


    public EffectReceipt PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys)
        => PulseKeys(pulseInfo, keys.Select(x => x.Key));

    public EffectReceipt PulseZones(PulseInfo pulseInfo, KeyboardZones zones)
    {
        colorController.ThrowIfDisconnected();
        //

        throw new NotImplementedException();
    }

    // This creates a flicker pattern every 1.5T, the flicker is logaritmic
    /// <summary>
    /// sin(  log( tan(x) ) * pi^( sin( x^2 )  )
    /// </summary>
    private static readonly WaveFunction _flickerModulation = x =>
        Math.Sin(
            Math.Log( Math.Tan(x) ) * Math.Pow(
                Math.PI, Math.Sin(Math.Pow(x, 2)
                ))
            );
    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys)
        => PulseKeys(pulseInfo with { WaveModulation = _flickerModulation }, keys);

    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys)
        => PulseKeys(pulseInfo with { WaveModulation = _flickerModulation }, keys);
    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, params KeyboardKey[] keys)
        => PulseKeys(pulseInfo with { WaveModulation = _flickerModulation }, keys);

    public EffectReceipt FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys)
        => PulseKeys(pulseInfo with { WaveModulation = _flickerModulation }, keys);

    public EffectReceipt FlickerZones(PulseInfo pulseInfo, KeyboardZones zones)
        => PulseZones(pulseInfo with { WaveModulation = _flickerModulation }, zones);

    public EffectReceipt FlashKeys(FlashInfo flashInfo, params KeyboardKeys[] keys)
    {
        colorController.ThrowIfDisconnected();

        throw new NotImplementedException();
    }

    public EffectReceipt FlashKeys(FlashInfo pulseInfo, params KeyboardKey[] keys)
    {
        colorController.ThrowIfDisconnected();

        throw new NotImplementedException();
    }

    public EffectReceipt FlashZones(FlashInfo pulseInfo, KeyboardZones zones)
    {
        colorController.ThrowIfDisconnected();

        throw new NotImplementedException();
    }

    public void StopEffectOnKeys(params KeyboardKeys[] keys)
    {
        colorController.ThrowIfDisconnected();
        _receiptHandler.RelinquishAccess(keys);
    }

    public void StopEffectOnKeys(params KeyboardKey[] keys)
    {
        colorController.ThrowIfDisconnected();
        _receiptHandler.RelinquishAccess(keys.Select(x => x.Key));
    }

    public void StopEffectOnZones(KeyboardZones zones)
    {
        colorController.ThrowIfDisconnected();

        var keys = ZoneUtility.GetKeysFromZone(zones);
        _receiptHandler.RelinquishAccess(keys);
    }
}
