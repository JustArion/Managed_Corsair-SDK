using System.Diagnostics;

namespace Dawn.CorsairSDK.Rewrite;

using Threading;

public record DeviceReconnectPolicy(bool Reconnect, int MaxRetries, TimeSpan RetryTimeout)
{
    internal AtomicInteger _retries;
    internal int _lastRetryTickCount;

    public static readonly DeviceReconnectPolicy NoReconnect = new(false, default, default);
    public static readonly DeviceReconnectPolicy Default = new(true, 5, TimeSpan.FromSeconds(1));
    public static readonly DeviceReconnectPolicy Infinite = new(true, -1, TimeSpan.FromSeconds(1));

    public CancellationToken GetMaxWaitTime()
    {
        var time = MaxRetries * RetryTimeout;
        Debug.WriteLine($"Max Wait Time is {time}", "Reconnection Policy");
        return new CancellationTokenSource(time).Token;
    }
}
