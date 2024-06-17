namespace Corsair.Threading;

public struct AtomicInteger
{
    public int GetAndSet(int newValue) => Interlocked.Exchange(ref _value, newValue);

    /// <summary>Compares two 32-bit signed integers for equality and, if they are equal, replaces the first value.</summary>
    /// <param name="value">The value that replaces the destination value if the comparison results in equality.</param>
    /// <param name="comparand">The value that is compared to the value at <paramref name="location1"/>.</param>
    /// <returns>The original value in <paramref name="location1"/>.</returns>
    /// <exception cref="NullReferenceException">The address of <paramref name="location1"/> is a null pointer.</exception>
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

    /// <summary>Increments a specified variable and stores the result, as an atomic operation.</summary>
    /// <param name="location">The variable whose value is to be incremented.</param>
    /// <returns>The incremented value.</returns>
    /// <exception cref="NullReferenceException">The address of location is a null pointer.</exception>
    public int IncrementAndGet() => Interlocked.Increment(ref _value);

    /// <summary>Decrements a specified variable and stores the result, as an atomic operation.</summary>
    /// <param name="location">The variable whose value is to be decremented.</param>
    /// <returns>The decremented value.</returns>
    /// <exception cref="NullReferenceException">The address of location is a null pointer.</exception>
    public int DecrementAndGet() => Interlocked.Decrement(ref _value);

    private volatile int _value;
}
