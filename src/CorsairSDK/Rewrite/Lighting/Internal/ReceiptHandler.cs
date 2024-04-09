using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;

namespace Dawn.CorsairSDK.Rewrite.Lighting.Internal;

public class ReceiptHandler<T> where T : notnull
{
    private readonly Dictionary<T, IDisposable> _allReceipts = new();

    public IEnumerable<T> Get(Dictionary<T, IDisposable> clientReceipts)
    {
        foreach (var (key, value) in clientReceipts)
            if (HasValidReceipt(key, value))
                yield return key;
    }
    public IDisposable Set(IEnumerable<T> resources, Func<T, IDisposable> resourceDisposedBuilder)
    {
        var clientReceipts = new Dictionary<T, IDisposable>();
        foreach (var resource in resources)
        {
            var disposeOperation = resourceDisposedBuilder(resource);
            lock (_allReceipts)
                _allReceipts[resource] = disposeOperation;

            clientReceipts.Add(resource, disposeOperation);
        }

        return Disposable.Create(new Receipts<T>(clientReceipts), CompareReceipts);
    }

    public Receipts<T> SetAndGetReceipts(IEnumerable<T> resources, Func<T, IDisposable> resourceDisposedBuilder)
    {
        var clientReceipt = new Dictionary<T, IDisposable>();
        foreach (var resource in resources)
        {
            var disposeOperation = resourceDisposedBuilder(resource);
            lock (_allReceipts)
                _allReceipts[resource] = disposeOperation;

            clientReceipt.Add(resource, disposeOperation);
        }

        return new(clientReceipt);
    }


    public void DisposeAndClear()
    {
        lock (_allReceipts)
        {
            foreach (var (_, value) in _allReceipts)
                value.Dispose();

            _allReceipts.Clear();
        }
    }
    public void Clear()
    {
        lock (_allReceipts)
            _allReceipts.Clear();
    }

    public bool HasValidReceipt(T obj, IDisposable receipt)
        => Internal_HasValidReceipt(obj, receipt, true);

    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    private bool Internal_HasValidReceipt(T obj, IDisposable receipt, bool shouldLock)
    {
        if (!shouldLock)
            return _allReceipts.TryGetValue(obj, out var dispo) && dispo.Equals(receipt);

        lock (_allReceipts)
            return _allReceipts.TryGetValue(obj, out var disposable) && disposable.Equals(receipt);

    }
    public void CompareReceipts(Receipts<T> clientReceipts)
    {
        lock (_allReceipts)
            foreach (var (key, value) in clientReceipts._underlyingReceipt)
            {
                if (!Internal_HasValidReceipt(key, value, false))
                    continue;

                value.Dispose();
                _allReceipts.Remove(key);
            }
    }
    public void RelinquishAccess(IEnumerable<T> objects)
    {
        lock (_allReceipts)
            foreach (var obj in objects)
            {
                if (!_allReceipts.TryGetValue(obj, out var disposable))
                    continue;

                try
                {
                    disposable.Dispose();
                }
                finally
                {
                    _allReceipts.Remove(obj);
                }
            }
    }

}
