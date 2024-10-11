using System.Diagnostics;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Animations;

/// <summary>
/// A base class for creating and managing lighting animations filled with useful methods and events
/// </summary>
public abstract class LightingAnimation : IAnimation
{
    private bool _isPaused;
    private TimeSpan _duration;
    protected bool _positionsInverted;

    protected abstract IGrouping<float, KeyValuePair<int, LedInfo>>[] Positions { get; init; }
    protected readonly ManualResetEventSlim _pauseResetEvent;

    protected bool IsPlaying { get; private set; }

    protected LightingAnimation()
    {
        // Initial State is unpaused
        _pauseResetEvent = new(true);
        Started += OnStarted;
        Ended += OnEnded;
        Resumed += OnResumed;
        Paused += OnPaused;
        Errored += OnErrored;

        Started += delegate { IsPlaying = true; };
        Resumed += delegate { IsPlaying = true; };
        Ended += delegate { IsPlaying = false; };
        Paused += delegate { IsPlaying = false; };
    }

    private void UpdateAnimationState(bool isPaused)
    {
        if (isPaused)
            _pauseResetEvent.Reset();
        else
            _pauseResetEvent.Set();
    }

    protected virtual void OnStarted(object? sender, EventArgs e) { }
    protected virtual void OnEnded(object? sender, EventArgs e) { }
    protected virtual void OnResumed(object? sender, EventArgs e) { }
    protected virtual void OnPaused(object? sender, EventArgs e) { }
    protected virtual void OnErrored(object? sender, Exception e) { }

    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            FPS = (int)(Positions.Length / Math.Clamp(_duration.TotalSeconds, 1, int.MaxValue));
            Debug.WriteLine($"FPS Set to '{FPS}'", "Animation");
        }
    }
    public TimeSpan CurrentTime { get; protected set; }
    public int FPS { get; protected set; }
    public bool Loop { get; set; }
    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            if (!(_isPaused ^ value))
                return;

            _isPaused = value;
            UpdateAnimationState(value);
            Debug.WriteLine($"Animation is now {(value ? "paused" : "unpaused")}", "Animation");
        }
    }


    // During the animations, the contents of the array may be reversed. To restore an animation, the contents should be revertable.
    protected virtual void ReversePositions()
    {
        Array.Reverse(Positions);
        _positionsInverted = !_positionsInverted;
    }

    public virtual void Reverse() => ReversePositions();

    public abstract Task Play();

    public virtual void Pause()
    {
        IsPaused = true;
        Paused(this, EventArgs.Empty);
    }

    public virtual void Resume()
    {
        IsPaused = false;
        Resumed(this, EventArgs.Empty);
    }

    public virtual void Stop()
    {
        IsPaused = false;
        Ended(this, EventArgs.Empty);
    }

    public virtual async Task Restart()
    {
        Stop();

        await Play();
    }

    protected static void RaiseStarted(LightingAnimation animation) => animation.Started.Invoke(animation, EventArgs.Empty);
    protected static void RaiseEnded(LightingAnimation animation) => animation.Ended.Invoke(animation, EventArgs.Empty);
    protected static void RaisePaused(LightingAnimation animation) => animation.Paused.Invoke(animation, EventArgs.Empty);
    protected static void RaiseResumed(LightingAnimation animation) => animation.Resumed.Invoke(animation, EventArgs.Empty);
    protected static void RaiseErrored(LightingAnimation animation, Exception exception) => animation.Errored.Invoke(animation, exception);

    protected event EventHandler<Exception> Errored;
    public event EventHandler Started;
    public event EventHandler Ended;
    public event EventHandler Paused;
    public event EventHandler Resumed;

    public abstract void Dispose();
}
