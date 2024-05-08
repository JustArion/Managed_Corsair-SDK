namespace Corsair.Connection.Internal;

using Contracts;

internal class DeviceReconnectHandler : IDeviceReconnectHandler
{
    public DeviceReconnectPolicy ReconnectPolicy { get; set; } = DeviceReconnectPolicy.Default;
    private const int MIN_RECONNECT_MS = 5;

    public bool HasValidReconnectPolicy() => RetryTimeoutHasPassed() && HasRetriesRemaining(ReconnectPolicy._retries.Value);

    private bool HasRetriesRemaining(int retries) => retries < ReconnectPolicy.MaxRetries;

    private bool RetryTimeoutHasPassed()
    {
        var policy = ReconnectPolicy;
        return Environment.TickCount - policy._lastRetryTickCount >= policy.RetryTimeout.TotalMilliseconds;
    }

    private int GetRemainingTimeMs() => (int)Math.Clamp(ReconnectPolicy.RetryTimeout.TotalMilliseconds - (Environment.TickCount - ReconnectPolicy._lastRetryTickCount), MIN_RECONNECT_MS, int.MaxValue);

    public async Task RequestReconnection(Func<bool> reconnectionAttempt)
    {
        // var policy = ReconnectPolicy;

        if (!ReconnectPolicy.Reconnect)
            return;

        var retries = ReconnectPolicy._retries;
        if (!HasRetriesRemaining(retries.Value))
            return;

        do
        {
            if (!RetryTimeoutHasPassed())
                await Task.Delay(GetRemainingTimeMs());

            if (!HasRetriesRemaining(retries.GetAndIncrement()))
                break;

        } while (ReconnectFailed(reconnectionAttempt));


    }

    private bool ReconnectFailed(Func<bool> reconnectionAttempt)
    {
        ReconnectPolicy._lastRetryTickCount = Environment.TickCount;
        return !reconnectionAttempt();
    }
}
