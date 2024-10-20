using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Corsair.Device.Internal.Contracts;
using Corsair.Lighting.Animations.Internal;
using Corsair.Lighting.Contracts;
using Corsair.Threading;
using Corsair.Threading.Internal;
using OneOf.Types;

namespace Corsair.Lighting.Animations;

/// <exception cref="T:Corsair.Exceptions.DeviceNotConnectedException">The device is not connected, the operation could not be completed</exception>
/// <exception cref="T:Corsair.Exceptions.CorsairException">An unexpected event happened, the device may have gotten disconnected</exception>
public sealed class ProgressAnimation : LightingAnimation
{
    private readonly ProgressAnimationOptions _options;
    private readonly IKeyboardLighting _keyboardLighting;
    private AtomicBoolean _shouldStop;

    protected override IGrouping<float, KeyValuePair<int, LedInfo>>[] Positions { get; init; }

    public ProgressAnimation(ProgressAnimationOptions options) : this(options, CorsairSDK.KeyboardLighting) { }

    public ProgressAnimation(ProgressAnimationOptions options, IKeyboardLighting keyboardLighting)
    {
        _options = options;
        _keyboardLighting = keyboardLighting;
        var positions = keyboardLighting.Colors.NativeInterop.GetPositionInfo();

        var keys = options.Keys.ToHashSet();
        var keyInfos = positions.Where(x => keys.Contains((KeyboardKeys)x.Key)).ToArray();

        var axisGroup = keyInfos.OrderBy(x =>
                options.IsVertical
                    ? x.Value.Position.Y
                    : x.Value.Position.X)
            .GroupBy(x =>
                options.IsVertical
                    ? x.Value.Position.Y
                    : x.Value.Position.X);

        Positions = (options.StartPosition is StartingPosition.LeftToRight or StartingPosition.TopToBottom
            ? axisGroup
            : axisGroup.Reverse())
            .ToArray();
    }


    /* Legend:
     * [] - Insignificant State
     * [x] - Key is lit
     * [-] - Key is unlit
     * ---
     *
     * Okay there's a multitude of things that need to happen here for the animation to work as intended.
     * 1) The plan is to have the selected keys shift in the direction of the Progress % value
     * eg. Frames:
     * --- Initial
     * [][][][][]
     * --- First Change
     * [x][][][][]
     * --- Second Change
     * [x][x][][][]
     * ... etc
     *
     * 2) After shifting, the strobed keys should have some timeout (eg. it only strobes once every 5sec)
     *
     * 3) The final key that the Progress % is at is pulsing on and off
     * eg. Frames:
     * --- Initial
     * [][][][][x]
     * --- First Change
     * [][][][][-]
     * --- Second Change
     * [][][][][x]
     * ... etc
     */
    public override async Task Play()
    {
        IsPaused = false;
        var colorController = _keyboardLighting.Colors;
        using (await DeviceAnimationSemaphore.WaitAsync(colorController.Device))
        {
            Debug.WriteLine("Playing Scan Animation", "Animation");
            var interop = colorController.NativeInterop;
            var thisFPS = FPS;
            var frameWaitTime = GetFrameWaitTimeMS(thisFPS);
            var startTime = Environment.TickCount64;
            long interopDuration = 0;
            Progress = _options.InitialProgress;
            RaiseStarted(new LightingAnimationState(frameWaitTime, DateTimeOffset.Now - TimeSpan.FromMilliseconds(startTime), this));

            // Loop

            // If we're stopping, stop, but also reset the _shouldStop bool
            while (!_shouldStop.CompareAndSet(true, false))
            {
                for (var iPos = 0; iPos < Positions.Length; iPos++)
                {
                    // Wait if paused
                    await _pauseResetEvent.WaitAsync();
                    if (_shouldStop)
                        break;

                    _percentFlashReceipt = null; // remove
                    // We can use pulses with No wave modulation to fade from black -> color

                    await SyncAnimationTime(iPos, frameWaitTime, startTime);
                }
            }

            // Cleanups
            _percentFlashReceipt?.Dispose();
            var endTime = Environment.TickCount64;
            Debug.WriteLine("Finished", "Animation");

            RaiseEnded();
            OnAnimationEnd(TimeSpan.FromMilliseconds(endTime - startTime), frameWaitTime, thisFPS, interopDuration);
        }
    }

    private KeyboardKey GetPercentageKey()
    {
        throw new NotImplementedException();
    }

    private EffectReceipt? _percentFlashReceipt;
    private byte _progress;
    public byte Progress
    {
        get => _progress;
        set
        {
            _progress = Math.Clamp(value, (byte)0, (byte)100);
            _percentFlashReceipt?.Dispose();

            if (!IsPlaying)
                return;

            var flashColor = _options.PercentFlashColor == default ? Color.White : _options.PercentFlashColor;
            _percentFlashReceipt = _keyboardLighting.Effects.FlashKeys(new FlashInfo(flashColor, TimeSpan.FromSeconds(0.8)),
                GetPercentageKey());
        }
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private async Task SyncAnimationTime(int iPos, int frameWaitTime, long startTime)
    {
        // Adds on the previous Laps' progress to the CurrentTime
        // This makes each lap not start at 00:00
        CurrentTime = TimeSpan.FromMilliseconds((Positions.Length + iPos) * frameWaitTime);
        // FPS
        await Task.Delay(frameWaitTime);

        #if LOG_PROGRESS
        Debug.WriteLine($"\t{TimeSpan.FromMilliseconds(Environment.TickCount64 - startTime):g} | {frameWaitTime} | {CurrentTime:g}/{Duration:g} | Reversed: {_positionsInverted}", "Animation");
        #endif
    }

    private static int GetFrameWaitTimeMS(int targetFPS) => throw new NotImplementedException();

    // Just some composition here ;)
    [Conditional("DEBUG")]
    private void OnAnimationEnd(TimeSpan actualDuration, int frameTime, int fps, long interopTime)
    {
        Debug.WriteLine(new string('-', 40), "Animation");
        Debug.WriteLine("Animation Stats", "Animation");
        Debug.WriteLine(new string('-', 40), "Animation");
        var frameLoss = actualDuration - Duration;
        WriteStat("Frame Time", $"{frameTime}ms");
        WriteStat("FPS", fps);
        if (frameLoss.TotalMilliseconds > 0)
        {
            WriteStat("C-Frame Loss", $"{frameLoss.TotalMilliseconds - interopTime}ms"); // Frame loss due to managed code
            WriteStat("I-Frame Loss", $"{interopTime}ms"); // Frame loss due to interop
            WriteStat("Total Frame Loss", $"{frameLoss.TotalMilliseconds}ms");
        }
        WriteStat("End Time", CurrentTime);
        WriteStat("Ideal Duration", Duration);
        WriteStat("Actual Duration", actualDuration);
        Debug.WriteLine(new string('-', 40), "Animation");
    }

    [Conditional("DEBUG")]
    private void WriteStat<T>(string statName, T stat)
        => Debug.WriteLine($"{statName, -17}| {stat}", "Animation");

    public override void Dispose()
    {
        Stop();
        ClearControlledColors(_keyboardLighting.Colors.NativeInterop);
        GC.SuppressFinalize(this);
    }

    ~ProgressAnimation() => Dispose();
}
