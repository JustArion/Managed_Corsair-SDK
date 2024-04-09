namespace Dawn.CorsairSDK.Rewrite.Lighting.Internal;

public struct Receipts<T> where T : notnull
{
    internal readonly Dictionary<T, IDisposable> _underlyingReceipt;

    internal Receipts(Dictionary<T, IDisposable> underlyingReceipt ) => _underlyingReceipt = underlyingReceipt;
}
