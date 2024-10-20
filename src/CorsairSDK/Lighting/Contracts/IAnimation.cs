using Corsair.Lighting.Animations;

namespace Corsair.Lighting.Contracts;

public interface IAnimation : IDisposable
{
    TimeSpan Duration { get; set; }
    TimeSpan CurrentTime { get; }

    int FPS { get; }

    bool Loop { get; set; }

    bool IsPaused { get; set; }

    void Reverse();

    Task Play();
    void Pause();
    void Resume();
    void Stop();
    Task Restart();

    event EventHandler<LightingAnimationState> Started;
    event EventHandler Ended;
    event EventHandler Paused;
    event EventHandler Resumed;
}
