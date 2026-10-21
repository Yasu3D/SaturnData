using System;
using System.Globalization;
using SaturnData.Content.Serialization;

namespace SaturnData.Content.Cosmetics.NavigatorData;

/// <summary>
/// Describes the base size of a navigator.
/// </summary>
public class NavigatorSize : ISctSerializable
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

    public string Serialize(ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return "";
        
        return $"{Width.ToString(CultureInfo.InvariantCulture),-4} {Height.ToString(CultureInfo.InvariantCulture),-4}";
    }

    public void Deserialize(string[] data, ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return;
        if (data.Length != 1) return;
            
        string[] split = data[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 2) return;
        
        try
        {
            Width = Convert.ToSingle(split[0], CultureInfo.InvariantCulture);
            Height = Convert.ToSingle(split[1], CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}