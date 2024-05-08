namespace Corsair.Lighting;

using System.Drawing;
using System.Numerics;

public record KeyboardKey(KeyboardKeys Key, Vector2 Coordinate)
{
    internal readonly int Id = (int)Key;
    public Color Color { get;  set; }
}
