using System;
using System.Globalization;
using SaturnData.Content.Serialization;

namespace SaturnData.Content.Cosmetics.NavigatorData;

/// <summary>
/// Describes the position offset of a navigator.
/// </summary>
public class NavigatorOffset : ISctSerializable
{
    /// <param name="horizontal">The horizontal offset in pixels.</param>
    /// <param name="vertical">The vertical offset in pixels.</param>
    public NavigatorOffset(float horizontal, float vertical)
    {
        Horizontal = horizontal;
        Vertical = vertical;
    }

    public NavigatorOffset()
    {
        Horizontal = 0;
        Vertical = 0;
    }
    
    /// <summary>
    /// The horizontal offset to apply to the in-game UI element.
    /// </summary>
    /// <remarks>
    /// - Negative values move UI element left.<br/>
    /// - Positive values move UI element right.
    /// </remarks>
    public float Horizontal;
    
    /// <summary>
    /// The vertical offset to apply to the in-game UI element.
    /// </summary>
    /// <remarks>
    /// - Negative values move UI element down.<br/>
    /// - Positive values move UI element up.
    /// </remarks>
    public float Vertical;

    public string Serialize(ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return "";
        
        return $"{Horizontal.ToString(CultureInfo.InvariantCulture),-4} {Vertical.ToString(CultureInfo.InvariantCulture),-4}";
    }

    public void Deserialize(string[] data, ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return;
        if (data.Length != 1) return;
            
        string[] split = data[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 2) return;
        
        try
        {
            Horizontal = Convert.ToSingle(split[0], CultureInfo.InvariantCulture);
            Vertical = Convert.ToSingle(split[1], CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}