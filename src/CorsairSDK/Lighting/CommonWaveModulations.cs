using System.Diagnostics.CodeAnalysis;

namespace Corsair.Lighting;

public delegate double WaveFunction(float x);
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class CommonWaveModulations
{
    public static readonly WaveFunction None = f => f;

    public static readonly WaveFunction Sine = f => Math.Sin(f);

    public static readonly WaveFunction Sawtooth = f => f % 1;

    public static readonly WaveFunction Triangle = f => Math.Abs((f % 2) - 1);

    public static readonly WaveFunction Square = f => f % 1 < 0.5 ? 1 : 0;

    // This creates a flicker pattern every 1.5T, the flicker is logaritmic
    /// <summary>
    /// sin(  log( tan(x) ) * pi^( sin( x^2 )  )
    public static readonly WaveFunction Flicker = x =>
        Math.Sin(
            Math.Log( Math.Tan(x) ) * Math.Pow(
                Math.PI, Math.Sin(Math.Pow(x, 2)
                ))
        );
}

