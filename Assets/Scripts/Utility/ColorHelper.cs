using System.Collections.Generic;
using UnityEngine;

public class ColorHelper
{
    public static List<Color> Colors = new List<Color>() { Color.black, Color.blue, Color.red, Color.green, Color.magenta, Color.yellow };

    public static Color GetColor(int index)
    {
        return Colors[index];
    }
}