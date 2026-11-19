using System;
using System.Globalization;
using SaturnData.Content.Serialization;
using SaturnData.Notation.Core;

namespace SaturnData.Content.StageUp;

/// <summary>
/// A song to play in stage-up mode.
/// </summary>
public class StageUpSong : ISctSerializable
{
    /// <summary>
    /// The <see cref="Entry.Id"/> of the song entry to play.
    /// </summary>
    public string EntryId { get; set; } = "";
    
    /// <summary>
    /// Should the song be displayed as <c>???</c> on the stage select and song preview screen?
    /// </summary>
    public bool Secret { get; set; } = false;

    public string Serialize(ContentFormatVersion formatVersion)
    {
        return $"{(Secret ? "SECRET " : "VISIBLE")} {EntryId}";
    }

    public void Deserialize(string[] data, ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return;
        if (data.Length != 1) return;
            
        string[] split = data[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 2) return;
        
        try
        {
            Secret = split[0] == "SECRET";
            EntryId = split[1];
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}