﻿namespace Corsair.Lighting;

using System.Drawing;
using Contracts;
using Internal;

public static class Extensions
{
    public static void SetColorFromId(this IKeyboardColorController colorController, int ledId, Color color)
    {
        if (colorController is not KeyboardColorController controller)
            throw new NotSupportedException();

        controller._lighting.SetLedColor(ledId, color);
    }

    public static void ClearColorFromId(this IKeyboardColorController colorController, int ledId)
    {
        if (colorController is not KeyboardColorController controller)
            throw new NotSupportedException();

        controller._lighting.SetLedColor(ledId, Color.Black);
    }
}
