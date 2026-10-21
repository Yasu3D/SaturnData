using System;
using System.Globalization;
using SaturnData.Content.Serialization;

namespace SaturnData.Content.Cosmetics.NavigatorData;

/// <summary>
/// Describes the distance from the edges of the base UI element, to the edges of the face UI element.
/// </summary>
public class NavigatorMargin : ISctSerializable
{
    /// <summary>
    /// The distance from the left edge of the main UI element to the left edge of the face UI element.
    /// </summary>
    public float Left;
    
    /// <summary>
    /// The distance from the top edge of the main UI element to the top edge of the face UI element.
    /// </summary>
    public float Top;
    
    /// <summary>
    /// The distance from the right edge of the main UI element to the right edge of the face UI element.
    /// </summary>
    public float Right;
    
    /// <summary>
    /// The distance from the bottom edge of the main UI element to the bottom edge of the face UI element.
    /// </summary>
    public float Bottom;
    
    /// <param name="left">The left edge margin in pixels.</param>
    /// <param name="top">The top edge margin in pixels.</param>
    /// <param name="right">The right edge margin in pixels.</param>
    /// <param name="bottom">The bottom edge margin in pixels.</param>
    public NavigatorMargin(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public string Serialize(ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return "";
        
        return $"{Left.ToString(CultureInfo.InvariantCulture),-4} {Top.ToString(CultureInfo.InvariantCulture),-4} {Right.ToString(CultureInfo.InvariantCulture),-4} {Bottom.ToString(CultureInfo.InvariantCulture),-4}";
    }

    public void Deserialize(string[] data, ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return;
        if (data.Length != 1) return;
        
        string[] split = data[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 4) return;
        
        try
        {
            Left = Convert.ToSingle(split[0], CultureInfo.InvariantCulture);
            Top = Convert.ToSingle(split[1], CultureInfo.InvariantCulture);
            Right = Convert.ToSingle(split[2], CultureInfo.InvariantCulture);
            Bottom = Convert.ToSingle(split[3], CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}