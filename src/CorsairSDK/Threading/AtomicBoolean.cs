namespace Corsair.Threading;

public struct AtomicBoolean
{
    public bool GetAndSet(bool newValue) => Convert.ToBoolean(_value.GetAndSet(Convert.ToInt32(newValue)));

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
