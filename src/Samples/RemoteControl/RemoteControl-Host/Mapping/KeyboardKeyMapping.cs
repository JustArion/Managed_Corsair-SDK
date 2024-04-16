namespace Dawn.Apps.RemoteControlHost.Mapping;

using System.Drawing;
using System.Numerics;
using CorsairSDK.Rewrite;
using CorsairSDK.Rewrite.Lighting;

public static class KeyboardKeyMapping
{
    public static KeyboardKey MapToKeyboardKey(this KeyboardState message)
    {
        return new KeyboardKey((KeyboardKeys)message.Key.Id, message.Coordinate.MapToVector2())
        {
            Color = message.Color.MapToColor(),
        };
    }
    public static KeyboardState MapToKeyboardKeyMessage(this KeyboardKey key)
    {
        return new KeyboardState
        {
            Key = new Dawn.Apps.RemoteControlHost.KeyboardKeys { Id = (int)key.Key},
            Color = key.Color.MapToColorMessage(),
            Coordinate = key.Coordinate.MapToVector2Message(),
        };
    }

    public static Dawn.Apps.RemoteControlHost.Color MapToColorMessage(this Color color)
    {
        return new Dawn.Apps.RemoteControlHost.Color
        {
            A = color.A,
            R = color.R,
            G = color.G,
            B = color.B,
        };
    }

    public static Dawn.Apps.RemoteControlHost.Vector2 MapToVector2Message(this Vector2 vector2) => new()
    {
        X = vector2.X, Y = vector2.Y
    };
    
    public static Color MapToColor(this Dawn.Apps.RemoteControlHost.Color color) 
        => Color.FromArgb(color.A, color.R, color.G, color.B);

    public static Vector2 MapToVector2(this Dawn.Apps.RemoteControlHost.Vector2 vector2) 
        => new(vector2.X, vector2.Y);
}