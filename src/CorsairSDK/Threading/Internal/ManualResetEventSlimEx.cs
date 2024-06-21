namespace Corsair.Threading.Internal;

internal static class ManualResetEventSlimEx
{
    public static Task WaitAsync(this ManualResetEventSlim resetEvent) => WaitHandleAsyncFactory.FromWaitHandle(resetEvent.WaitHandle);
}
