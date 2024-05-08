namespace Corsair.Lighting.Contracts;

public interface IAnimation
{
    TimeSpan Duration { get; }
    TimeSpan CurrentTime { get; }

    bool Loop { get; set; }

    bool IsPaused { get; set; }

    void Play();
    void Pause();
    void Stop();
    void Restart();

    event EventHandler Started;
    event EventHandler Ended;
    event EventHandler Paused;
    event EventHandler Resumed;

}
