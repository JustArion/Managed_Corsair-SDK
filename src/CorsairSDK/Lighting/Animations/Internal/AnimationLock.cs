using System.Diagnostics;
using Corsair.Threading;

namespace Corsair.Lighting.Animations.Internal;

internal abstract class AnimationLock : IDisposable
{
    private AtomicBoolean _disposed;
    private readonly TaskCompletionSource _tcs = new();

    public void Release() => Dispose();

    public void Wait() => _tcs.Task.Wait();
    public Task WaitAsync() => _tcs.Task;

    protected abstract void OnDispose();

    public void Dispose()
    {
        if (!_disposed.CompareAndSet(false, true))
            return;

        OnDispose();
        _tcs.TrySetResult();
    }
}

internal sealed class DisposableAnimationLock(Action onDispose) : AnimationLock
{
    protected override void OnDispose() => onDispose();
}

internal sealed class DisposableAnimationLock<T>(Action<T> onDispose, T state) : AnimationLock
{
    protected override void OnDispose() => onDispose(state);
}

