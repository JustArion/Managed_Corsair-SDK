using Corsair.Threading;
using Corsair.Threading.Internal;

namespace Corsair.Lighting.Animations.Internal;

using System.Diagnostics;
using System.Drawing;
using Contracts;

public class ScanAnimation : LightingAnimation
{
    private readonly ScanAnimationOptions _options;
    private readonly  IKeyboardColorController _colorController;

    private AtomicBoolean _shouldStop;
    private bool _positions_inverted;

    public ScanAnimation(ScanAnimationOptions options, IKeyboardColorController colorController)
    {
        _options = options;
        _colorController = colorController;
        var positionInfos = _colorController.NativeInterop.GetPositionInfo();

        // X = Horizontal
        // We get groupings based on the X or Y axis of the keyboard
        // If X we get all the Columns of the keyboard keys
        // If Y we get all the Rows of the keyboard keys
        var axisGroup = positionInfos.OrderBy(x =>
            options.IsVertical
            ? x.Value.Position.Y
            : x.Value.Position.X)
            .GroupBy(x =>
                options.IsVertical
                ? x.Value.Position.Y
                : x.Value.Position.X);
        _positions = options.StartingPosition is StartingPosition.LeftToRight or StartingPosition.TopToBottom
            ? axisGroup.ToArray()
            : axisGroup.Reverse().ToArray();

        Duration = options.Duration;
    }

    protected override void OnPaused(object? sender, EventArgs e) => Debug.WriteLine($"{(_options.IsVertical ? "Vertical" : "Horizontal")} Scan Animation Paused", "Animation");

    protected override void OnResumed(object? sender, EventArgs e) => Debug.WriteLine($"{(_options.IsVertical ? "Vertical" : "Horizontal")} Scan Animation Resumed", "Animation");

    protected override void OnEnded(object? sender, EventArgs e)
    {
        SetAllBlack();
        _colorController.ClearAll();

        Debug.WriteLine($"{(_options.IsVertical ? "Vertical" : "Horizontal")} Scan Animation Finished", "Animation");
    }

    protected override void OnStarted(object? sender, EventArgs e)
    {
        _colorController.ClearAll();

        Debug.WriteLine($"{(_options.IsVertical ? "Vertical" : "Horizontal")} Scan Animation Started", "Animation");
    }

    // During the animations, the contents of the array may be reversed. To restore an animation, the contents should be revertable.
    private void ReversePositions()
    {
        Array.Reverse(_positions);
        _positions_inverted = !_positions_inverted;
    }

    private static int GetFrameWaitTimeMS(int targetFPS) => 1000 / Math.Clamp(targetFPS, 1, int.MaxValue) / LAPS;

    // Left -> Right becomes Right -> Left (and vice-versa)
    private const int LAPS = 2;
    private const int KEYS_PEY_COLUMN = 6;

    public override void Reverse()
    {
        ReversePositions();
        SetAllBlack();
    }

    public override async Task Play()
    {
        if (IsPlaying)
        {
            Debug.WriteLine("Animation is already playing", "Animation");

            await Restart();
            return;
        }

        RaiseStarted(this);
        Debug.WriteLine("Playing Animation", "Animation");
        var interop = _colorController.NativeInterop;
        var thisFPS = FPS;
        var frameWaitTime = GetFrameWaitTimeMS(thisFPS);
        var startTime = Environment.TickCount64;
        long interopDuration = 0;

        // A -> B & B -> A
        for (var i = 0; Loop || i < LAPS; i++)
        {
            for (var iPos = 0; iPos < _positions.Length; iPos++)
            {
                // Wait if paused
                await _pauseResetEventSlim.WaitAsync();
                // Stop if requested
                if (_shouldStop)
                    break;

                var position = _positions[iPos];

                // We keep the last 6 keys since we can't determine what is a new column on the horizon.
                // Though we can estimate that the approx amount of keys from top - bottom is 6
                // We need to do this since we can't just group by X since the sizeof(Esc) > sizeof(Tab). The X position is based on the width of the key
                var erasePosition = iPos - KEYS_PEY_COLUMN;

                if (erasePosition >= 0)
                    foreach (var prevKeyId in _positions[erasePosition].Select(x => x.Key))
                    {
                        var interopStartTime = Environment.TickCount64;
                        interop.SetLedColor(prevKeyId, Color.Black);
                        interopDuration += Environment.TickCount64 - interopStartTime;
                    }

                var keys = position.Select(x => x.Key).ToArray();
                foreach (var keyId in keys)
                {
                    var interopStartTime = Environment.TickCount64;
                    interop.SetLedColor(keyId, _options.Color);
                    interopDuration += Environment.TickCount64 - interopStartTime;
                }

                // Adds on the previous Laps' progress to the CurrentTime
                // This makes each lap not start at 00:00
                CurrentTime = TimeSpan.FromMilliseconds(((_positions.Length * i) + iPos) * frameWaitTime);
                // FPS
                await Task.Delay(frameWaitTime / LAPS);

                Debug.WriteLine($"\t{TimeSpan.FromMilliseconds(Environment.TickCount64 - startTime):g} | {frameWaitTime} | {CurrentTime:g}/{Duration:g} | Reversed: {_positions_inverted}", "Animation");
            }

            if (_shouldStop.CompareAndSet(true, false))
                break;
            // Left -> Right becomes Right -> Left (and vice-versa)
            ReversePositions();
        }
        var endTime = Environment.TickCount64;
        Debug.WriteLine("Finished", "Animation");

        RaiseEnded(this);

        OnAnimationEnd(TimeSpan.FromMilliseconds(endTime - startTime), frameWaitTime, thisFPS, interopDuration);
    }

    private void OnAnimationEnd(TimeSpan actualDuration, int frameTime, int fps, long interopTime)
    {
        Debug.WriteLine("Preparing Animation Stats", "Animation");
        Debug.WriteLine(new string('-', 40), "Animation");
        var frameLoss = actualDuration - Duration;
        WriteStat("Frame Time", $"{frameTime}ms");
        WriteStat("FPS", fps);
        if (frameLoss.TotalMilliseconds > 0)
        {
            WriteStat("C-Frame Loss", $"{frameLoss.TotalMilliseconds - interopTime}ms");
            WriteStat("Total Frame Loss", $"{frameLoss.TotalMilliseconds}ms");
        }
        WriteStat("End Time", CurrentTime);
        WriteStat("Ideal Duration", Duration);
        WriteStat("Actual Duration", actualDuration);
    }

    private void WriteStat<T>(string statName, T stat)
        => Debug.WriteLine($"{statName, -17}| {stat}", "Animation");

    private void SetAllBlack()
    {
        var interop = _colorController.NativeInterop;
        var keys = _positions.SelectMany(x => x.Select(y => y.Key));
        foreach (var key in keys)
            interop.SetLedColor(key, Color.Black);
    }

    public override void Stop()
    {
        Debug.WriteLine("Stopping Animation");
        _shouldStop.CompareAndSet(false, true);

        base.Stop();
    }

    public override async Task Restart()
    {
        Debug.WriteLine("Restarting Animation");
        Stop();

        // We reset the array to its original state
        if (_positions_inverted)
            ReversePositions();

        await Play();
    }

    public override void Dispose()
    {
        SetAllBlack();
        GC.SuppressFinalize(this);
    }
}
