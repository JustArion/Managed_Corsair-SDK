namespace Dawn.CorsairSDK.Rewrite.Lighting.Contracts;

using System.Drawing;
using Rewrite;

public interface IColorController : IDisposable
{
    KeyboardKey[] KeyboardKeys { get; }

    IDisposable SetFromBitmap(byte[] bitmap);

    /// <returns>The lifetime of the key being set</returns>
    IDisposable SetKeys(Color color, params KeyboardKeys[] keys);

    IDisposable SetKeys(Color color, params KeyboardKey[] keys);
    void ClearKeys(params KeyboardKeys[] keys);

    /// <returns>The lifetime of the key being set</returns>
    IDisposable SetZones(Color color, KeyboardZones zones);
    void ClearZones(KeyboardZones zones);


    /// <returns>The lifetime of the key being set</returns>
    IDisposable SetMultiple(params KeyboardKeyColor[] keyColors);
    void ClearMultiple(params KeyboardKeyColor[] keyColors);

    /// <summary>
    /// An alias for SetZones(KeyboardZones.AllZones, color);
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    IDisposable SetGlobal(Color color);
    void ClearAll();
}
