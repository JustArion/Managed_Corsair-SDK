namespace Corsair.Lighting.Contracts;

public interface IAnimation
{
    TimeSpan Duration { get; set; }
    TimeSpan CurrentTime { get; }

    public byte FPS { get; }

    bool Loop { get; set; }

    bool IsPaused { get; set; }

    Task Play();
    void Pause();
    void Stop();
    Task Restart();

    event EventHandler Started;
    event EventHandler Ended;
    event EventHandler Paused;
    event EventHandler Resumed;

}
