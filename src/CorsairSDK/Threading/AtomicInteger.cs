namespace Corsair.Threading;

public struct AtomicInteger
{
    public int GetAndSet(int newValue) => Interlocked.Exchange(ref _value, newValue);

    public bool CompareAndSet(int expected, int newValue) => Interlocked.CompareExchange(ref _value, newValue, expected) == expected;

    public bool SetIfLessThan(int newValue)
    {
        while (true)
        {
            var value = _value;
            if (value >= newValue)
                break;
            if (CompareAndSet(value, newValue))
                return true;
        }
        return false;
    }

    public int Value
    {
        get => _value;
        set => _value = value;
    }

    public int GetAndIncrement() => IncrementAndGet() - 1;

    public int IncrementAndGet() => Interlocked.Increment(ref _value);

    public int DecrementAndGet() => Interlocked.Decrement(ref _value);

    private volatile int _value;
}
