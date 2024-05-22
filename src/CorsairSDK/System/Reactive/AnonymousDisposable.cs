// The following code is from dotnet. I do not claim the following as my own. The same goes for anything under the System namespace within the project.
// https://github.com/dotnet/reactive/blob/main/Rx.NET/Source/src/System.Reactive/Disposables/AnonymousDisposable.cs
// -----

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace System.Reactive.Disposables;

/// <summary>
/// Represents an Action-based disposable.
/// </summary>
internal sealed class AnonymousDisposable : ICancelable
{
    private volatile Action? _dispose;

    /// <summary>
    /// Constructs a new disposable with the given action used for disposal.
    /// </summary>
    /// <param name="dispose">Disposal action which will be run upon calling Dispose.</param>
    public AnonymousDisposable(Action dispose)
    {
        Diagnostics.Debug.Assert(dispose != null);

        _dispose = dispose;
    }

    /// <summary>
    /// Gets a value that indicates whether the object is disposed.
    /// </summary>
    public bool IsDisposed => _dispose == null;

    /// <summary>
    /// Calls the disposal action if and only if the current instance hasn't been disposed yet.
    /// </summary>
    public void Dispose()
    {
        Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }
}

/// <summary>
/// Represents a Action-based disposable that can hold onto some state.
/// </summary>
internal sealed class AnonymousDisposable<TState> : ICancelable
{
    private TState _state;
    private volatile Action<TState>? _dispose;

    /// <summary>
    /// Constructs a new disposable with the given action used for disposal.
    /// </summary>
    /// <param name="state">The state to be passed to the disposal action.</param>
    /// <param name="dispose">Disposal action which will be run upon calling Dispose.</param>
    public AnonymousDisposable(TState state, Action<TState> dispose)
    {
        Diagnostics.Debug.Assert(dispose != null);

        _state = state;
        _dispose = dispose;
    }

    /// <summary>
    /// Gets a value that indicates whether the object is disposed.
    /// </summary>
    public bool IsDisposed => _dispose == null;

    /// <summary>
    /// Calls the disposal action if and only if the current instance hasn't been disposed yet.
    /// </summary>
    public void Dispose()
    {
        Interlocked.Exchange(ref _dispose, null)?.Invoke(_state);
        _state = default!;
    }
}
