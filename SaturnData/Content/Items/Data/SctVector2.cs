using System;
using System.Globalization;
using SaturnData.Content.Serialization;

namespace SaturnData.Content.Items.Data;

public class SctVector2 : ISctSerializable
{
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    public SctVector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public SctVector2()
    {
        X = 0;
        Y = 0;
    }

    /// <summary>
    /// The X component.
    /// </summary>
    public float X;

    /// <summary>
    /// The Y component.
    /// </summary>
    public float Y;

    public string Serialize(ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return "";

        return $"{X.ToString(CultureInfo.InvariantCulture),-4} {Y.ToString(CultureInfo.InvariantCulture),-4}";
    }

    public void Deserialize(string[] data, ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return;
        if (data.Length != 1) return;

        string[] split = data[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 2) return;

        try
        {
            X = Convert.ToSingle(split[0], CultureInfo.InvariantCulture);
            Y = Convert.ToSingle(split[1], CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}