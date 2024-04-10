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


    public IDisposable PulseKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys) => PulseKeys(pulseInfo, (IEnumerable<KeyboardKeys>)keys);

    public IDisposable PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys)
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
        Task.Run(()=> DoPulseKeys(pulseInfo, controlledKeys, cts.Token));

        return Disposable.Create(disposables, list => {
            foreach (var disposable in list)
                disposable?.Dispose();
        });
    }

    public IDisposable PulseKeys(PulseInfo pulseInfo, params KeyboardKey[] keys)
        => PulseKeys(pulseInfo, keys.Select(x => x.Key));

    private async Task DoPulseKeys(PulseInfo pulseInfo, List<KeyboardKeys> controlledKeys, CancellationToken token)
    {
        var startTime = Environment.TickCount;
        var intervalMs = pulseInfo.Interval.TotalMilliseconds;

        while (!token.IsCancellationRequested || PulseIsOccurring(pulseInfo, startTime))
        {
            var deltaTime = Environment.TickCount - startTime;

            var period = Math.Clamp(intervalMs, 1, double.MaxValue);

            var progress = (float)(deltaTime / period);

            // X: Time | Y : Color
            var (waveX, waveY) = CalculateWave(pulseInfo, progress);

            var lerpedColor = ColorEffects.LerpColor(pulseInfo.Start, pulseInfo.End, waveX, waveY);

            if (Math.Round(progress, 2) % 0.5 == 0)
            {
                string color;
                if (lerpedColor is { R: 255, G: 0 })
                    color = "Red";
                else if (lerpedColor.G == Color.Green.G)
                    color = "Green";
                else color = $"R:{lerpedColor.R} G: {lerpedColor.G}";

                Trace.WriteLine($"X (Time): {Math.Round(progress, 2)}\t Y (Color): {Math.Round(waveX, 2)}   \t| {deltaTime / 1000}s \t| {color}");
            }


            colorController.SetKeys(lerpedColor, controlledKeys);
            await Task.Delay(1, token);
        }
    }

    private static (float WaveX, float WaveY) CalculateWave(PulseInfo info, float x)
    {
        if (info.Modulation == null)
            return (x, x);


        // x = y | /

        var waveLength = info.Modulation.WaveLength(x, x);
        var waveAmplitude = info.Modulation.WaveAmplitude?.Invoke(x, x) ?? x;

        return ((float WaveX, float WaveY))(waveLength, waveAmplitude);

    }

    private static bool PulseIsOccurring(PulseInfo pulseInfo, int startTime) =>
        pulseInfo.IsInfinite // Infinity
        || Environment.TickCount - startTime < pulseInfo.TotalDuration.TotalMilliseconds; // Duration hasn't been reached yyet


    public IDisposable PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys)
        => PulseKeys(pulseInfo, keys.Select(x => x.Key));

    public IDisposable PulseZones(PulseInfo pulseInfo, KeyboardZones zones)
    {
        colorController.ThrowIfDisconnected();
        //

        throw new NotImplementedException();
    }

    private static readonly PulseModulation _flickerModulation = new((x, _) => Math.Sin(x * Math.PI), (_, _) => 7);
    public IDisposable FlickerKeys(PulseInfo pulseInfo, params KeyboardKeys[] keys)
        => PulseKeys(pulseInfo with { Modulation = _flickerModulation }, keys);

    public IDisposable FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKeys> keys)
        => PulseKeys(pulseInfo with { Modulation = _flickerModulation }, keys);
    public IDisposable FlickerKeys(PulseInfo pulseInfo, params KeyboardKey[] keys)
        => PulseKeys(pulseInfo with { Modulation = _flickerModulation }, keys);

    public IDisposable FlickerKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys)
        => PulseKeys(pulseInfo with { Modulation = _flickerModulation }, keys);

    public IDisposable FlickerZones(PulseInfo pulseInfo, KeyboardZones zones)
        => PulseZones(pulseInfo with { Modulation = _flickerModulation }, zones);

    public IDisposable FlashKeys(FlashInfo flashInfo, params KeyboardKeys[] keys)
    {
        colorController.ThrowIfDisconnected();

        throw new NotImplementedException();
    }

    public IDisposable FlashKeys(FlashInfo pulseInfo, params KeyboardKey[] keys)
    {
        colorController.ThrowIfDisconnected();

        throw new NotImplementedException();
    }

    public IDisposable FlashZones(FlashInfo pulseInfo, KeyboardZones zones)
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
