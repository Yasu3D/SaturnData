using System;
using System.Globalization;
using SaturnData.Content.Serialization;

namespace SaturnData.Content.Cosmetics.NavigatorData;

/// <summary>
/// Describes the base size of a navigator.
/// </summary>
public struct NavigatorSize : ISctSerializable
{
    /// <param name="width">The width of the in-game UI element in pixels.</param>
    /// <param name="height">The height of the in-game UI element in pixels.</param>
    public NavigatorSize(float width, float height)
    {
        Width = width;
        Height = height;
    }
    
    /// <summary>
    /// The width of the in-game UI element in pixels.
    /// </summary>
    public float Width;
    
    /// <summary>
    /// The height of the in-game UI element in pixels.
    /// </summary>
    public float Height;

    public string Serialize()
    {
        return $"{Width.ToString(CultureInfo.InvariantCulture),-4} {Height.ToString(CultureInfo.InvariantCulture),-4}";
    }

    public static NavigatorSize Deserialize(string[] data)
    {
        if (data.Length != 1) return new NavigatorSize();
            
        string[] split = data[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 2) return new NavigatorSize();

        float width = 0;
        float height = 0;
        
        try
        {
            width = Convert.ToSingle(split[0], CultureInfo.InvariantCulture);
            height = Convert.ToSingle(split[1], CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return new NavigatorSize(width, height);
    }
}