using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Async;
using Corsair.Device;
using Corsair.Lighting.Internal;

namespace Corsair.Lighting.Animations.Internal;

internal static class DeviceAnimationSemaphore
{
    private static readonly ConcurrentDictionary<string, AnimationLock> _locks = new();

    /// <summary>
    /// Wait's if the current device is under a lock. Locks the device until released.
    /// </summary>
    /// <returns>A lock object that releases the device lock when disposed</returns>
    public static async Task<AnimationLock> WaitAsync(CorsairDevice device)
    {
        if (_locks.TryGetValue(device.Id, out var animationLock))
            await animationLock.WaitAsync();

        var deviceAnimationLock = new DisposableAnimationLock<CorsairDevice>(OnDispose, device);
        _locks.TryAdd(device.Id, deviceAnimationLock);
        return deviceAnimationLock;
    }

    private static void OnDispose(CorsairDevice dev)
    {
        Debug.WriteLine($"Animation lock released for {dev.Type.ToString().ToLower()} device '{dev.Model}'", nameof(DeviceAnimationSemaphore));
        _locks.TryRemove(dev.Id, out _);
    }
}
