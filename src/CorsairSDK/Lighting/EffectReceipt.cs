using Corsair.Threading;

namespace Corsair.Lighting;

public class EffectReceipt : IDisposable
{
    private readonly Task _task;
    private readonly IDisposable _disposable;
    private AtomicBoolean _disposed;

    internal EffectReceipt(Task task, IDisposable disposable)
    {
        _task = task ?? throw new NullReferenceException(nameof(task));
        _disposable = disposable ?? throw new NullReferenceException(nameof(disposable));
    }

    public void CancelAfter(TimeSpan ts) => _task.Wait((int)ts.TotalMilliseconds);

    /// <summary>
    /// Will wait for the effect to complete (if infinite, you may be waiting a while ;) )
    /// </summary>
    public TaskAwaiter GetAwaiter() => _task.GetAwaiter();

    /// <summary>
    /// Will immediately stop the effect
    /// </summary>
    public void Dispose()
    {
        if (!_disposed.CompareAndSet(false, true))
            return;

        _disposable.Dispose();
        GC.SuppressFinalize(this);
    }

    ~EffectReceipt() => Dispose();
}
