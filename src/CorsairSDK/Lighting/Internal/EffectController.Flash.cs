using System.Diagnostics;
using System.Drawing;
using System.Reactive.Disposables;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Internal;

// public record FlashInfo(Color Color, TimeSpan SingleFlashDuration, TimeSpan FlashPeakDuration, bool IsInfinite, TimeSpan EffectDuration = default);
internal partial class EffectController
{
    public EffectReceipt FlashKeys(FlashInfo flashInfo, IEnumerable<KeyboardKeys> keys)
    {
        VerifyBounds(flashInfo);
        colorController.ThrowIfDisconnected();

        var array = keys.ToArray();

        var disposables = new List<IDisposable?>();
        var cts = new CancellationTokenSource();
        var controlledKeys = new List<KeyboardKeys>(array);

        var effectKeysReceipt = _receiptHandler.Set(array, keyboardKey => Disposable.Create(keyboardKey, key => {
            lock (controlledKeys)
            {
                controlledKeys.Remove(key);
                colorController.ClearKeys(key);
                Debug.WriteLine($"Clearing Key: {key}",$"{nameof(FlashKeys)}({nameof(FlashInfo)}) {nameof(flashInfo)}, {nameof(KeyboardKeys)}[] {nameof(keys)}");

                if (controlledKeys.Count == 0)
                    cts.Cancel();
            }
        }));

        disposables.Add(effectKeysReceipt);
        //
        disposables.Add(Disposable.Create(cts, source => {
            if (!source.IsCancellationRequested)
                source.Cancel();
        }));

        // ReSharper disable once MethodSupportsCancellation
        var flashTask = Task.Run(()=> DoKeyFlashes(flashInfo, controlledKeys, cts.Token));

        var disposable = Disposable.Create(disposables, list => {
            foreach (var disposable in list)
                disposable?.Dispose();
        });

        return new EffectReceipt(flashTask, disposable);
    }

    private void VerifyBounds(FlashInfo flashInfo)
    {
        if (flashInfo.SingleFlashDuration == default)
            throw new ArgumentException($"{nameof(flashInfo)}.{nameof(flashInfo.SingleFlashDuration)} cannot be default. An effect requires a flash interval", nameof(flashInfo));

        if (flashInfo.FlashPeakDuration > flashInfo.SingleFlashDuration)
            throw new ArgumentException(
                $"{nameof(flashInfo)}.{nameof(flashInfo.FlashPeakDuration)} cannot be greater than {nameof(flashInfo)}.{nameof(flashInfo.SingleFlashDuration)}",
                nameof(flashInfo));

        if (flashInfo.IsInfinite)
            return;

        if (flashInfo.EffectDuration == default)
            throw new ArgumentException(
                $"{nameof(flashInfo)}.{nameof(flashInfo.EffectDuration)} cannot be default. An effect requires a duration (or Infinite)",
                nameof(flashInfo));

        if (flashInfo.FlashPeakDuration > flashInfo.EffectDuration)
            throw new ArgumentException(
                $"{nameof(flashInfo)}.{nameof(flashInfo.FlashPeakDuration)} cannot be greater than {nameof(flashInfo)}.{nameof(flashInfo.EffectDuration)}",
                nameof(flashInfo));
    }
    // public record FlashInfo(Color Color, TimeSpan SingleFlashDuration, TimeSpan FlashPeakDuration, bool IsInfinite, TimeSpan EffectDuration = default);
    // Flash Interval:  [------------[Flash Duration]]
    //                                                  v -- Fade Out Duration
    // Fade Out Duration: [------------[Flash Duration]---]
    // Effect Duration: [------------[Flash Duration]][------------[Flash Duration]] (For 2 flashes)
    // TODO: IMPLEMENT THE FADE OUT DURATION. It should be a modifier on the flashPercent. Reducing it from the FlashInterval.
    // TODO: Remaining Flash Interval = Flash Duration - Fade Out Duration
    private async Task DoKeyFlashes(FlashInfo flashInfo, List<KeyboardKeys> controlledKeys, CancellationToken token)
    {
        var startTime = Environment.TickCount;
        var intervalMs = flashInfo.SingleFlashDuration.TotalMilliseconds;
        var flashDurationMS = flashInfo.FlashPeakDuration.TotalMilliseconds;
        var flashPercent = 1 - (flashDurationMS / intervalMs);

        while (!token.IsCancellationRequested && FlashIsOccurring(flashInfo, startTime))
        {
            var deltaTime = Environment.TickCount - startTime;

            var period = Math.Clamp(intervalMs, 1, double.MaxValue); // Prevent Div by 0

            var progress = Math.Clamp((float)(deltaTime / period), 0, 1);

            if (progress >= flashPercent)
                colorController.SetKeys(flashInfo.Color, controlledKeys);
            else
            {
                var lerpColor = ColorEffects.LerpColor(Color.Black, flashInfo.Color, progress);

                #if DEBUG
                Trace.WriteLine($"{deltaTime / 1000}s \t| {lerpColor}");
                #endif

                colorController.SetKeys(lerpColor, controlledKeys);
            }

            await Task.Delay(1, token);
            if (progress >= 1)
                startTime = Environment.TickCount;
        }
    }

    private static bool FlashIsOccurring(FlashInfo flashInfo, int startTime)
    {
        if (flashInfo.IsInfinite)
            return true;

        var durationPassed = Environment.TickCount - startTime;
        return durationPassed < flashInfo.EffectDuration.TotalMilliseconds;
    }

    public EffectReceipt FlashKeys(FlashInfo flashInfo, params KeyboardKeys[] keys)
        => FlashKeys(flashInfo, (IEnumerable<KeyboardKeys>)keys);

    public EffectReceipt FlashKeys(FlashInfo flashInfo, params KeyboardKey[] keys)
        => FlashKeys(flashInfo, keys.Select(x => x.Key));

    public EffectReceipt FlashKeys(FlashInfo flashInfo, IEnumerable<KeyboardKey> keys)
        => FlashKeys(flashInfo, keys.Select(x => x.Key));

    public EffectReceipt FlashZones(FlashInfo pulseInfo, KeyboardZones zones)
    {
        var keys = ZoneUtility.GetKeysFromZones(zones);

        return FlashKeys(pulseInfo, keys);
    }
}
