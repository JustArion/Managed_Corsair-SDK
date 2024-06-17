namespace Corsair.Threading;

public struct AtomicBoolean
{
    public bool GetAndSet(bool newValue) => Convert.ToBoolean(_value.GetAndSet(Convert.ToInt32(newValue)));

    /// <summary>Compares two booleans for equality and, if they are equal, replaces the first value.</summary>
    /// <param name="value">The value that replaces the destination value if the comparison results in equality.</param>
    /// <param name="comparand">The value that is compared to the value at <paramref name="location1"/>.</param>
    /// <returns>The original value in <paramref name="location1"/>.</returns>
    /// <exception cref="NullReferenceException">The address of <paramref name="location1"/> is a null pointer.</exception>
    public bool CompareAndSet(bool expected, bool newValue) => _value.CompareAndSet(Convert.ToInt32(expected), Convert.ToInt32(newValue));

    public bool Value
    {
        get => Convert.ToBoolean(_value.Value);
        set => _value.Value = Convert.ToInt32(value);
    }

    public static bool operator true(AtomicBoolean boolean) => boolean.Value;

    public static bool operator false(AtomicBoolean boolean) => !boolean.Value;

    private AtomicInteger _value;
}
