namespace System.Reactive;

public interface ICancelable : IDisposable
{
    bool IsDisposed { get; }
}
