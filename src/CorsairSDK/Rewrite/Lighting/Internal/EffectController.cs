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

            var progress = deltaTime / period;

            var (wavePercent, waveAmplitude) = GetWaveInfo(pulseInfo, (float)progress);

            var lerpedColor = ColorEffects.LerpColor(pulseInfo.Start, pulseInfo.End, (float)wavePercent, waveAmplitude);

            if (Math.Round(progress, 2) % 0.5 == 0)
                Trace.WriteLine($"Time: {Math.Round(progress, 2)} Color: {Math.Round(wavePercent, 2)} | {deltaTime / 1000}s | R:{lerpedColor.R} G: {lerpedColor.G}");


            colorController.SetKeys(lerpedColor, controlledKeys);
            await Task.Delay(1, token);

            // ORIGINAL
            // Debug.WriteLine($"[{string.Join(", ", controlledKeys)}] | {pulseInfo.Start} -> {pulseInfo.End}", "Pulse Zones");
            // await ColorEffects.FadeTo(pulseInfo.Start, pulseInfo.End, pulseInfo.PulseModulation.Interval / 2, color => colorController.SetKeys(color, controlledKeys), token);
            //
            // Debug.WriteLine($"[{string.Join(", ", controlledKeys)}] | {pulseInfo.End} -> {pulseInfo.Start}", "Pulse Zones");
            // await ColorEffects.FadeTo(pulseInfo.End, pulseInfo.Start, pulseInfo.PulseModulation.Interval / 2, color => colorController.SetKeys(color, controlledKeys), token);
        }
    }

    private static (double WaveX, float WaveY) GetWaveInfo(PulseInfo info, float x) =>
        info.Modulation == null
            ? (x, PulseModulation.DEFAULT_AMPLITUDE)
            : (info.Modulation.WaveLength(x), info.Modulation.WaveAmplitude);

    private static bool PulseIsOccurring(PulseInfo pulseInfo, int startTime) =>
        pulseInfo.IsInfinite // Infinity
        || Environment.TickCount - startTime < pulseInfo.TotalDuration.TotalMilliseconds; // Duration hasn't been reached yyet


    public IDisposable PulseKeys(PulseInfo pulseInfo, IEnumerable<KeyboardKey> keys)
        => PulseKeys(pulseInfo, keys.Select(x => x.Key));

    public IDisposable PulseZones(PulseInfo pulseInfo, KeyboardZones zones)
    {
        colorController.ThrowIfDisconnected();
        //
        // // PulseInfo(Color Start, Color End, TimeSpan PulseDuration, TimeSpan PulseInterval, bool IsInfinite);
        // Task.Run(async () => {
        //     var startTime = Environment.TickCount;
        //
        //     while (pulseInfo.IsInfinite ||
        //            Environment.TickCount - startTime < pulseInfo.PulseDuration.TotalMilliseconds)
        //     {
        //
        //         Debug.WriteLine($"{zones} | {pulseInfo.Start} -> {pulseInfo.End}", "Pulse Zones");
        //         // PulseInterval is halfed since the amplitude is the middle of the pulse interval, not the end
        //         // So 1 pulse is Start -> End -> Start
        //         // Instead of Start -> End
        //         await ColorEffects.FadeTo(pulseInfo.Start, pulseInfo.End, pulseInfo.PulseInterval / 2, color => colorController.SetZones(color, zones));
        //
        //         Debug.WriteLine($"{zones} | {pulseInfo.End} -> {pulseInfo.Start}", "Pulse Zones");
        //         await ColorEffects.FadeTo(pulseInfo.End, pulseInfo.Start, pulseInfo.PulseInterval / 2, color => colorController.SetZones(color, zones));
        //
        //     }
        //
        //     colorController.SetZones(pulseInfo.End, zones);
        // });
        //
        // return Disposable.Empty;
        throw new NotImplementedException();
    }

    private static readonly PulseModulation _flickerModulation = new(x => Math.Sin(x * Math.PI), 7);
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
