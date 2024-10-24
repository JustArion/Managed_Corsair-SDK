namespace System.Reactive.Disposables;

public static class Disposable
{
    public static IDisposable Empty => EmptyDisposable.Instance;

    private sealed class EmptyDisposable : IDisposable
    {
        public static readonly EmptyDisposable Instance = new();

        private EmptyDisposable() { }

        public void Dispose()
        {
            // nop
        }
    }
    public static IDisposable Create(Action dispose)
    {
        if (dispose == null)
        {
            throw new ArgumentNullException(nameof(dispose));
        }

        return new AnonymousDisposable(dispose);
    }

    public static IDisposable Create<TState>(TState state, Action<TState> dispose)
    {
        if (dispose == null)
        {
            throw new ArgumentNullException(nameof(dispose));
        }

        return new AnonymousDisposable<TState>(state, dispose);
    }
}
