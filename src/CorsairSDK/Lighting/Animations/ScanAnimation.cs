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

    private readonly bool _isVertical;

    protected sealed override IGrouping<float, KeyValuePair<int, LedInfo>>[] Positions { get; init; }

    /// <exception cref="T:Corsair.Exceptions.DeviceNotConnectedException">The device is not connected, the operation could not be completed</exception>
    /// <exception cref="T:Corsair.Exceptions.CorsairException">An unexpected event happened, the device may have gotten disconnected</exception>
    public ScanAnimation(ScanAnimationOptions options, IKeyboardColorController colorController)
    {
        _options = options;
        _colorController = colorController;
        var positionInfos = _colorController.NativeInterop.GetPositionInfo();

        _isVertical = options.StartPosition is StartingPosition.TopToBottom or StartingPosition.BottomToTop;

        // X = Horizontal
        // We get groupings based on the X or Y axis of the keyboard
        // If X we get all the Columns of the keyboard keys
        // If Y we get all the Rows of the keyboard keys
        var axisGroup = positionInfos.OrderBy(x =>
                _isVertical
                    ? x.Value.Position.Y
                    : x.Value.Position.X)
            .GroupBy(x =>
                _isVertical
                    ? x.Value.Position.Y
                    : x.Value.Position.X);
        Positions = options.StartPosition is StartingPosition.LeftToRight or StartingPosition.TopToBottom
            ? axisGroup.ToArray()
            : axisGroup.Reverse().ToArray();

        Duration = options.Duration;
    }

    protected override void OnPaused(object? sender, EventArgs e) => Debug.WriteLine($"{(_isVertical ? "Vertical" : "Horizontal")} Scan Animation Paused", "Animation");

    protected override void OnResumed(object? sender, EventArgs e) => Debug.WriteLine($"{(_isVertical ? "Vertical" : "Horizontal")} Scan Animation Resumed", "Animation");

    protected override void OnEnded(object? sender, EventArgs e)
    {
        SetAllBlack();
        _colorController.ClearAll();

        Debug.WriteLine($"{(_isVertical ? "Vertical" : "Horizontal")} Scan Animation Finished", "Animation");
    }


    protected override void OnStarted(object? sender, EventArgs e)
    {
        _colorController.ClearAll();

        Debug.WriteLine($"{(_isVertical ? "Vertical" : "Horizontal")} Scan Animation Started", "Animation");
    }

    private static int GetFrameWaitTimeMS(int targetFPS) => 1000 / Math.Clamp(targetFPS, 1, int.MaxValue) / LAPS;

    // Left -> Right becomes Right -> Left (and vice-versa)
    private const int LAPS = 2;
    private const int KEYS_PEY_COLUMN = 6;

    public override void Reverse()
    {
        base.Reverse();
        SetAllBlack();
    }

    protected override void OnErrored(object? sender, Exception e)
    {
        Debug.WriteLine($"Animation Errored: {e.Message}", "Animation");

        IsPaused = false;
        Stop();

    }

    // The semaphore will wait for another animation to play and finish first.
    /* If the user does something like
     * -----------------------
     * instance.Play();
     * await instance.Play();
     *-----------------------
     * The second await will also wait for the first animation to finish due to the DeviceAnimationSemaphore
     */
    public override async Task Play()
    {
        IsPaused = false;
        using (await DeviceAnimationSemaphore.WaitAsync(_colorController.Device))
        {
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
                for (var iPos = 0; iPos < Positions.Length; iPos++)
                {
                    // Wait if paused
                    await _pauseResetEvent.WaitAsync();
                    // Stop if requested
                    if (_shouldStop)
                        break;


                    RemoveFillIfNeccessary(iPos, interop, ref interopDuration);

                    ScanNextPosition(iPos, interop, ref interopDuration);

                    await SyncAnimationTime(i, iPos, frameWaitTime, startTime);
                }

                if (_shouldStop.CompareAndSet(true, false))
                    break;
                // Left -> Right becomes Right -> Left (and vice-versa)
                Reverse();
            }
            var endTime = Environment.TickCount64;
            Debug.WriteLine("Finished", "Animation");

            RaiseEnded(this);

            OnAnimationEnd(TimeSpan.FromMilliseconds(endTime - startTime), frameWaitTime, thisFPS, interopDuration);
        }
    }

    private void ScanNextPosition(int iPos, ILightingInterop interop, scoped ref long interopDuration)
    {
        foreach (var keyId in Positions[iPos].Select(x => x.Key))
        {
            var interopStartTime = Environment.TickCount64;
            interop.SetLedColor(keyId, _options.Color);
            interopDuration += Environment.TickCount64 - interopStartTime;
        }
    }

    private async Task SyncAnimationTime(int i, int iPos, int frameWaitTime, long startTime)
    {
        // Adds on the previous Laps' progress to the CurrentTime
        // This makes each lap not start at 00:00
        CurrentTime = TimeSpan.FromMilliseconds(((Positions.Length * i) + iPos) * frameWaitTime);
        // FPS
        await Task.Delay(frameWaitTime / LAPS);

        #if LOG_PROGRESS
        Debug.WriteLine($"\t{TimeSpan.FromMilliseconds(Environment.TickCount64 - startTime):g} | {frameWaitTime} | {CurrentTime:g}/{Duration:g} | Reversed: {_positionsInverted}", "Animation");
        #endif
    }

    private void RemoveFillIfNeccessary(int iPos, ILightingInterop interop, scoped ref long interopDuration)
    {
        // We keep the last 6 keys since we can't determine what is a new column on the horizon.
        // Though we can estimate that the approx amount of keys from top - bottom is 6
        // We need to do this since we can't just group by X since the sizeof(Esc) > sizeof(Tab). The X position is based on the width of the key
        var erasePosition = iPos - KEYS_PEY_COLUMN;

        if (_options.Fill || erasePosition < 0)
            return;
        foreach (var prevKeyId in Positions[erasePosition].Select(x => x.Key))
        {
            var interopStartTime = Environment.TickCount64;
            interop.SetLedColor(prevKeyId, Color.Black);
            interopDuration += Environment.TickCount64 - interopStartTime;
        }
    }

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

    private void SetAllBlack()
    {
        var interop = _colorController.NativeInterop;
        var keys = Positions.SelectMany(x => x.Select(y => y.Key));
        foreach (var key in keys)
            interop.SetLedColor(key, Color.Black);
    }

    public override void Stop()
    {
        Debug.WriteLine("Stopping Animation");
        IsPaused = false;
        _shouldStop.CompareAndSet(false, true);

        // We reset the array to its original state
        if (_positionsInverted)
            ReversePositions();

        base.Stop();
    }

    public override async Task Restart()
    {
        Debug.WriteLine("Restarting Animation");

        await base.Restart();
    }

    public override void Dispose()
    {
        SetAllBlack();
        GC.SuppressFinalize(this);
    }
}
