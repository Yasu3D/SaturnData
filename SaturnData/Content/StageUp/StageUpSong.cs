using System;
using SaturnData.Notation.Core;

namespace SaturnData.Content.StageUp;

/// <summary>
/// A song to play in stage-up mode.
/// </summary>
[Serializable]
public class StageUpSong
{
    /// <summary>
    /// The <see cref="Entry.Id"/> of the song difficulty to play.
    /// </summary>
    public string EntryId { get; set; } = "";
    
    /// <summary>
    /// Should the song be displayed as <c>???</c> on the stage select and song preview screen?
    /// </summary>
    public bool Secret { get; set; } = false;
}