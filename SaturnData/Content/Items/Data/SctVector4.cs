using System;
using System.Globalization;
using SaturnData.Content.Serialization;

namespace SaturnData.Content.Items.Data;

public class SctVector4 : ISctSerializable
{
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    /// <param name="z">The Z component.</param>
    /// <param name="w">The W component.</param>
    public SctVector4(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public SctVector4()
    {
        X = 0;
        Y = 0;
        Z = 0;
        W = 0;
    }


    /// <summary>
    /// The X component.
    /// </summary>
    public float X;

    /// <summary>
    /// The Y component.
    /// </summary>
    public float Y;

    /// <summary>
    /// The Z component.
    /// </summary>
    public float Z;

    /// <summary>
    /// The W component.
    /// </summary>
    public float W;

    public string Serialize(ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return "";

        return $"{X.ToString(CultureInfo.InvariantCulture),-4} {Y.ToString(CultureInfo.InvariantCulture),-4} {Z.ToString(CultureInfo.InvariantCulture),-4} {W.ToString(CultureInfo.InvariantCulture),-4}";
    }

    public void Deserialize(string[] data, ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return;
        if (data.Length != 1) return;

        string[] split = data[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 4) return;

        try
        {
            X = Convert.ToSingle(split[0], CultureInfo.InvariantCulture);
            Y = Convert.ToSingle(split[1], CultureInfo.InvariantCulture);
            Z = Convert.ToSingle(split[2], CultureInfo.InvariantCulture);
            W = Convert.ToSingle(split[3], CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}