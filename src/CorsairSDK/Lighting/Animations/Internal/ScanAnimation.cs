using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Animations.Internal;

using System.Diagnostics;
using System.Drawing;
using Contracts;

public class ScanAnimation : IAnimation
{
    private readonly ScanAnimationOptions _options;
    private readonly  IKeyboardColorController _colorController;

    private readonly IGrouping<float, KeyValuePair<int, LedInfo>>[] _positions;
    private bool _positions_inverted;
    private TimeSpan _duration;

    public ScanAnimation(ScanAnimationOptions options, IKeyboardColorController colorController)
    {
        _options = options;
        _colorController = colorController;
        var positionInfos = colorController.NativeInterop.GetPositionInfo();

        // X = Horizontal
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

        Duration = DefaultDuration;

        Started += OnStarted;
        Ended += OnEnded;
        Resumed += OnResumed;
        Paused += OnPaused;
    }

    private void OnPaused(object? sender, EventArgs e)
    {
        Debug.WriteLine($"{(_options.IsVertical ? "Vertical" : "Horizontal")} Scan Animation Paused", "Animation");
    }

    private void OnResumed(object? sender, EventArgs e)
    {
        Debug.WriteLine($"{(_options.IsVertical ? "Vertical" : "Horizontal")} Scan Animation Resumed", "Animation");
    }

    private void OnEnded(object? sender, EventArgs e)
    {
        SetAllBlack();

        Debug.WriteLine($"{(_options.IsVertical ? "Vertical" : "Horizontal")} Scan Animation Finished", "Animation");
    }

    private void OnStarted(object? sender, EventArgs e)
    {
        _colorController.ClearAll();

        Debug.WriteLine($"{(_options.IsVertical ? "Vertical" : "Horizontal")} Scan Animation Started", "Animation");
    }

    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(3);

    // During the animations, the contents of the array may be reversed. To restore an animation, the contents should be revertable.
    private void ReverseArray()
    {
        Array.Reverse(_positions);
        _positions_inverted = !_positions_inverted;
    }

    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            FPS = (byte)(_positions.Length / _duration.TotalSeconds);
            Debug.WriteLine($"FPS Set to '{FPS}'");
        }
    }

    public TimeSpan CurrentTime { get; private set; }

    public byte FPS { get; private set; }

    private static int GetFrameWaitTimeMS(int targetFPS) => 1000 / Math.Clamp(targetFPS, 1, int.MaxValue) / LAPS;

    public bool Loop { get; set; }
    public bool IsPaused { get; set; }

    // Left -> Right becomes Right -> Left (and vice-versa)
    private const int LAPS = 2;
    private const int KEYS_PEY_COLUMN = 6;
    public async Task Play()
    {
        var thisFPS = FPS;
        var frameWaitTime = GetFrameWaitTimeMS(thisFPS);

        // A -> B & B -> A
        for (var i = 0; Loop || i < LAPS; i++)
        {
            for (var iPos = 0; iPos < _positions.Length; iPos++)
            {
                var position = _positions[iPos];

                // We keep the last 6 keys since we can't determine what is a new column on the horizon.
                // Though we can estimate that the approx amount of keys from top - bottom is 6
                // We need to do this since we can't just group by X since the sizeof(Esc) > sizeof(Tab). The X position is based on the width of the key
                var erasePosition = iPos - KEYS_PEY_COLUMN;

                if (erasePosition >= 0)
                    foreach (var prevKeyId in _positions[erasePosition].Select(x => x.Key))
                        _colorController.NativeInterop.SetLedColor(prevKeyId, Color.Black);

                var keys = position.Select(x => x.Key).ToArray();
                foreach (var keyId in keys)
                    _colorController.NativeInterop.SetLedColor(keyId, _options.Color);

                CurrentTime = TimeSpan.FromMilliseconds(iPos * frameWaitTime * LAPS);
                // FPS
                await Task.Delay(frameWaitTime);
                Debug.WriteLine($"{CurrentTime:g}/{Duration:g} | Reversed: {_positions_inverted}", "Animation");
            }

            // Left -> Right becomes Right -> Left (and vice-versa)
            ReverseArray();
        }
    }

    private void SetAllBlack()
    {
        foreach (var position in _positions)
        foreach (var key in position)
            _colorController.NativeInterop.SetLedColor(key.Key, Color.Black);
    }

    public void Pause() => IsPaused = true;

    public void Stop()
    {
    }

    public async Task Restart()
    {
        Stop();

        // We reset the array to its original state
        if (_positions_inverted)
            ReverseArray();

        await Play();
    }

    public event EventHandler Started;
    public event EventHandler Ended;
    public event EventHandler Paused;
    public event EventHandler Resumed;
}
