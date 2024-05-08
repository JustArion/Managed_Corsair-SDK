namespace Corsair.Lighting;

public readonly struct EffectReceipt : IDisposable
{
    private readonly Task _task;
    private readonly IDisposable _disposable;

    internal EffectReceipt(Task task, IDisposable disposable)
    {
        _task = task;
        _disposable = disposable;
    }

    /// <summary>
    /// Will wait for the effect to complete (if infinite, you may be waiting a while ;) )
    /// </summary>
    public TaskAwaiter GetAwaiter() => _task.GetAwaiter();

    /// <summary>
    /// Will immediately stop the effect
    /// </summary>
    public void Dispose() => _disposable.Dispose();
}
