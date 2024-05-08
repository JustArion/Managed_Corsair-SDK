namespace Corsair.Apps.RemoteControlHost.Mapping;

using System.Drawing;
using System.Numerics;
using Lighting;
using Dawn.Apps.RemoteControlHost;

public static class KeyboardKeyMapping
{
    public static KeyboardKey MapToKeyboardKey(this KeyboardStateMessage message)
    {
        return new KeyboardKey((KeyboardKeys)message.Key.Id, message.Coordinate.MapToVector2())
        {
            Color = message.Color.MapToColor(),
        };
    }
    public static KeyboardStateMessage MapToKeyboardKeyMessage(this KeyboardKey key)
    {
        return new KeyboardStateMessage
        {
            Key = new KeyboardKeysMessage() { Id = (int)key.Key},
            Color = key.Color.MapToColorMessage(),
            Coordinate = key.Coordinate.MapToVector2Message(),
        };
    }

    public static ColorMessage MapToColorMessage(this Color color)
    {
        return new ColorMessage
        {
            A = color.A,
            R = color.R,
            G = color.G,
            B = color.B,
        };
    }

    public static Vector2Message MapToVector2Message(this Vector2 vector2) => new()
    {
        X = vector2.X, Y = vector2.Y
    };
    
    public static Color MapToColor(this ColorMessage color) 
        => Color.FromArgb(color.A, color.R, color.G, color.B);

    public static Vector2 MapToVector2(this Vector2Message vector2) 
        => new(vector2.X, vector2.Y);
}