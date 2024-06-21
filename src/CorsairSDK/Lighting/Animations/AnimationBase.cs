using System.Diagnostics;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Animations;

public abstract class AnimationBase : IAnimation
{
    private bool _isPaused;
    private TimeSpan _duration;

    protected IGrouping<float, KeyValuePair<int, LedInfo>>[] _positions = [];
    protected readonly ManualResetEventSlim _pauseResetEventSlim;
    protected AnimationBase()
    {
        _pauseResetEventSlim = new(true);
        Started += OnStarted;
        Ended += OnEnded;
        Resumed += OnResumed;
        Paused += OnPaused;
    }

    private void UpdateAnimationState(bool isPaused)
    {
        if (isPaused)
            _pauseResetEventSlim.Reset();
        else
            _pauseResetEventSlim.Set();
    }

    protected virtual void OnStarted(object? sender, EventArgs e) { }
    protected virtual void OnEnded(object? sender, EventArgs e) { }
    protected virtual void OnResumed(object? sender, EventArgs e) { }
    protected virtual void OnPaused(object? sender, EventArgs e) { }

    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            FPS = (int)(_positions.Length / Math.Clamp(_duration.TotalSeconds, 1, int.MaxValue));
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

    public abstract void Reverse();

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

    protected static void RaiseStarted(AnimationBase animation) => animation.Started.Invoke(animation, EventArgs.Empty);
    protected static void RaiseEnded(AnimationBase animation) => animation.Ended.Invoke(animation, EventArgs.Empty);
    protected static void RaisePaused(AnimationBase animation) => animation.Paused.Invoke(animation, EventArgs.Empty);
    protected static void RaiseResumed(AnimationBase animation) => animation.Resumed.Invoke(animation, EventArgs.Empty);

    public event EventHandler Started;
    public event EventHandler Ended;
    public event EventHandler Paused;
    public event EventHandler Resumed;

    public abstract void Dispose();
}
